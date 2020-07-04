// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

public class MoveSystem : ComponentSystem {
    private Entity _inputEntity;
    private Entity _controlEntity; // TODO : choice move control entity 
    private BuildPhysicsWorld _buildPhysSystem;

    protected override void OnStartRunning() {
        Entities.ForEach((Entity inputEntity, ref InputDataComponent dataComp) => { _inputEntity = inputEntity; });
        Entities.ForEach((Entity controlEntity, ref MoveComponent moveComp) => { _controlEntity = controlEntity; });

        _buildPhysSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();
    }

    private bool IsLocked() {
        if (Entity.Null == _inputEntity) {
            return true;
        }

        if (false == EntityManager.HasComponent<InputDataComponent>(_inputEntity)) {
            return true;
        }

        if (Entity.Null == _controlEntity) {
            return true;
        }

        if (false == EntityManager.HasComponent<Translation>(_controlEntity)) {
            return true;
        }

        if (false == EntityManager.HasComponent<AnimationFrameComponent>(_controlEntity)) {
            return true;
        }

        if (EntityManager.HasComponent<AttackComponent>(_inputEntity)) {
            return true;
        }

        // TODO : other condition
        return false;
    }

    // TODO : temporary constant -> status 
    private const float _speedX = 0.03f;
    private const float _speedY = 0.1f;
    private const float _skinWidth = 0.01f;
    private const float _stepOffset = 0.01f;

    protected override void OnUpdate() {
        if (IsLocked()) {
            return;
        }

        var translation = EntityManager.GetComponentData<Translation>(_controlEntity);
        var animComp = EntityManager.GetComponentData<AnimationFrameComponent>(_controlEntity);

        var calcPos = translation.Value;
        calcPos.x = GetPositionX(calcPos);
        calcPos.y = GetPositionY(calcPos);

        var dir = calcPos - translation.Value;
        if (((0.0f < dir.y) && (_stepOffset < dir.y)) ||
            (0.0f > dir.y) && (-_stepOffset > dir.y)) {
            animComp.setId = Utility.AnimState.Jump;
            animComp.bLooping = false;
        }
        else if (((0.0f < dir.x) && (_stepOffset < dir.x)) ||
                 (0.0f > dir.x) && (-_stepOffset > dir.x)) {
            animComp.bFlipX = dir.x < 0.0f;
            animComp.setId = Utility.AnimState.Run;
            animComp.bLooping = true;
        }
        else {
            animComp.setId = Utility.AnimState.Idle;
            animComp.bLooping = true;
        }

        if (EntityManager.HasComponent<JumpComponent>(_controlEntity)) {
            if (false == EntityManager.HasComponent<AnimationLockComponent>(_controlEntity)) {
                EntityManager.AddComponentData(_controlEntity, new AnimationLockComponent(2));
            }
        }
        else {
            if (EntityManager.HasComponent<AnimationLockComponent>(_controlEntity)) {
                EntityManager.RemoveComponent<AnimationLockComponent>(_controlEntity);
            }
        }

        EntityManager.SetComponentData(_controlEntity, animComp);

        translation.Value = calcPos;
        EntityManager.SetComponentData(_controlEntity, translation);
    }

    private float GetPositionX(float3 inPos) {
        var moveComp = EntityManager.GetComponentData<MoveComponent>(_controlEntity);
        var velocity = float3.zero;
        velocity.x = moveComp.value.x;
        velocity.x *= _speedX;
        //Debug.Log("GetPosition----- X ---------");
        CollisionTest(velocity, ref inPos);
        return inPos.x;
    }

    private float GetPositionY(float3 inPos) {
        var moveComp = EntityManager.GetComponentData<MoveComponent>(_controlEntity);
        var velocity = float3.zero;
        velocity.y = moveComp.value.y;
        velocity.y *= Time.fixedDeltaTime;
        velocity.y *= _speedY;
        //Debug.Log("GetPosition----- Y ---------");
        CollisionTest(velocity, ref inPos);
        return inPos.y;
    }

    private unsafe void CollisionTest(float3 inVelocity, ref float3 outPos) {
        var physWorld = _buildPhysSystem.PhysicsWorld;
        var collider = EntityManager.GetComponentData<PhysicsCollider>(_controlEntity);
        var rotation = EntityManager.GetComponentData<Rotation>(_controlEntity);

        var startPos = outPos;
        var newPos = outPos + inVelocity;

        var bIsHit = physWorld.CastCollider(new ColliderCastInput {
            Collider = collider.ColliderPtr,
            Orientation = rotation.Value,
            Start = startPos,
            End = newPos
        }, out var hit);
        if (bIsHit) {
            newPos = math.lerp(startPos, newPos, hit.Fraction);
            newPos -= math.normalizesafe(inVelocity) * _skinWidth;
        }

        outPos = newPos;
    }
}