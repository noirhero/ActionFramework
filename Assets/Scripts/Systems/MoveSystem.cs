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
        Entities.ForEach((Entity inputEntity, ref InputDataComponent dataComp) => {
            _inputEntity = inputEntity;
        });

        Entities.ForEach((Entity controlEntity, ref MoveComponent moveComp) => {
            _controlEntity = controlEntity;
        });

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

    protected override void OnUpdate() {
        if (IsLocked()) {
            return;
        }

        var translation = EntityManager.GetComponentData<Translation>(_controlEntity);
        var calcPos = translation.Value;
        calcPos.x = GetPositionX(calcPos);
        calcPos.y = GetPositionY(calcPos);
        var dir = calcPos - translation.Value;

        var moveComp = EntityManager.GetComponentData<MoveComponent>(_controlEntity);
        var animComp = EntityManager.GetComponentData<AnimationFrameComponent>(_controlEntity);

        // run
        var bMovingX = moveComp.value.x != 0.0f;
        var bRunning = ((0.0f < dir.x) && (Utility.stepOffset < dir.x)) || (0.0f > dir.x) && (-Utility.stepOffset > dir.x);
        if (bMovingX || bRunning) {
            animComp.bFlipX = bMovingX ? moveComp.value.x < 0.0f : animComp.bFlipX;

            if (false == AnimUtility.HasState(animComp, AnimUtility.run)) {
                animComp.state |= AnimUtility.run;
            }
        }
        else {
            if (AnimUtility.HasState(animComp, AnimUtility.run)) {
                animComp.state ^= AnimUtility.run;
            }
        }

        // jump
        var bMovingY = moveComp.value.y != Utility.terminalVelocity;
        var bFalling = (0.0f > dir.y) && (-Utility.stepOffset > dir.y);
        var bJumping = (0.0f < dir.y) && (Utility.stepOffset < dir.y);
        if (bMovingY || bJumping || bFalling) {
            if (false == AnimUtility.HasState(animComp, AnimUtility.jump)) {
                animComp.state |= AnimUtility.jump;
            }
        }
        else {
            animComp.state |= AnimUtility.jump;

            if (AnimUtility.HasState(animComp, AnimUtility.jump)) {
                animComp.state ^= AnimUtility.jump;
            }
        }

        // jump anim lock
        if (bJumping) {
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

        if (0.0f != moveComp.value.x) {
            var velocity = float3.zero;
            velocity.x = moveComp.value.x;
            velocity.x *= Utility.speedX;

            //Debug.Log("GetPosition----- X ---------");
            CollisionTest(velocity, ref inPos);
        }

        return inPos.x;
    }

    private float GetPositionY(float3 inPos) {
        var moveComp = EntityManager.GetComponentData<MoveComponent>(_controlEntity);

        if (0.0f != moveComp.value.y) {
            var velocity = float3.zero;
            velocity.y = moveComp.value.y;
            velocity.y *= Time.fixedDeltaTime;
            velocity.y *= Utility.speedY;

            //Debug.Log("GetPosition----- Y ---------");
            CollisionTest(velocity, ref inPos);
        }

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
            newPos -= math.normalizesafe(inVelocity) * Utility.skinWidth;
        }

        outPos = newPos;
    }
}
