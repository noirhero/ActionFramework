// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

public class MoveSystem : ComponentSystem {
    private Entity _inputEntity;
    private Entity _controlEntity; // TODO : choice move control entity 
    private BuildPhysicsWorld _buildPhysSystem;

    protected override void OnStartRunning() {
        Entities.ForEach((Entity inputEntity, ref InputDataComponent dataComp) => { _inputEntity = inputEntity; });
        Entities.ForEach((Entity controlEntity, ref InputComponent inputComp) => { _controlEntity = controlEntity; });

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

        // TODO : other condition
        return false;
    }

    protected override void OnUpdate() {
        if (IsLocked()) {
            return;
        }

        bool bIsStop = true;
        if (TryMove()) {
            var animComp = EntityManager.GetComponentData<AnimationFrameComponent>(_controlEntity);
            animComp.setId = Utility.AnimState.Run;
            animComp.bLooping = true;
            EntityManager.SetComponentData(_controlEntity, animComp);

            bIsStop = false;
        }

        if (TryJump()) {
            var animComp = EntityManager.GetComponentData<AnimationFrameComponent>(_controlEntity);
            animComp.setId = Utility.AnimState.Jump;
            animComp.bLooping = false;
            EntityManager.SetComponentData(_controlEntity, animComp);

            bIsStop = false;
        }
        else if (TryFall()) {
            var animComp = EntityManager.GetComponentData<AnimationFrameComponent>(_controlEntity);
            animComp.setId = Utility.AnimState.Jump;
            animComp.bLooping = false;
            EntityManager.SetComponentData(_controlEntity, animComp);

            bIsStop = false;
        }

        if (bIsStop) {
            var animComp = EntityManager.GetComponentData<AnimationFrameComponent>(_controlEntity);
            animComp.setId = Utility.AnimState.Idle;
            animComp.bLooping = true;
            EntityManager.SetComponentData(_controlEntity, animComp);
        }
    }

    // TODO : temporary constant -> status 
    private const float _speedX = 0.03f;
    private const float _speedY = 0.2f;
    private const float _gravity = 1.0f;

    private bool TryMove() {
        if (EntityManager.HasComponent<MoveComponent>(_inputEntity)) {
            var moveComp = EntityManager.GetComponentData<MoveComponent>(_inputEntity);

            var transComp = EntityManager.GetComponentData<Translation>(_controlEntity);
            CollisionTest(ref transComp.Value, Vector3.right, moveComp.value.x);
            EntityManager.SetComponentData(_controlEntity, transComp);

            var animComp = EntityManager.GetComponentData<AnimationFrameComponent>(_controlEntity);
            animComp.bFlipX = moveComp.value.x < 0.0f;
            EntityManager.SetComponentData(_controlEntity, animComp);

            if (Utility.bShowInputLog) {
                Debug.Log("Move : " + moveComp.value.x + " (" + moveComp.accumTime + ")");
            }

            return true;
        }

        return false;
    }

    private bool TryJump() {
        if (EntityManager.HasComponent<JumpComponent>(_inputEntity)) {
            var jumpComp = EntityManager.GetComponentData<JumpComponent>(_inputEntity);
            jumpComp.force -= _gravity;
            EntityManager.SetComponentData(_inputEntity, jumpComp);

            bool bJumpDown = 0.0f > jumpComp.force;
            if (bJumpDown) {
                if (EntityManager.HasComponent<AnimationLockComponent>(_controlEntity)) {
                    EntityManager.RemoveComponent<AnimationLockComponent>(_controlEntity);
                }
            }
            else {
                if (false == EntityManager.HasComponent<AnimationLockComponent>(_controlEntity)) {
                    EntityManager.AddComponentData(_controlEntity, new AnimationLockComponent(2));
                }
            }

            var transComp = EntityManager.GetComponentData<Translation>(_controlEntity);
            var deltaY = jumpComp.force * Time.DeltaTime;
            if (CollisionTest(ref transComp.Value, Vector3.up, deltaY)) {
                EntityManager.RemoveComponent<JumpComponent>(_inputEntity);
                if (EntityManager.HasComponent<AnimationLockComponent>(_controlEntity)) {
                    EntityManager.RemoveComponent<AnimationLockComponent>(_controlEntity);
                }
            }

            EntityManager.SetComponentData(_controlEntity, transComp);

            if (Utility.bShowInputLog) {
                Debug.Log("Jump force (" + jumpComp.force + ")");
            }

            return true;
        }

        return false;
    }

    private bool TryFall() {
        if (EntityManager.HasComponent<FallComponent>(_inputEntity)) {
            var fallComp = EntityManager.GetComponentData<FallComponent>(_inputEntity);
            fallComp.force -= _gravity;
            var deltaY = fallComp.force * Time.DeltaTime;
            EntityManager.SetComponentData(_inputEntity, fallComp);

            var transComp = EntityManager.GetComponentData<Translation>(_controlEntity);
            if (CollisionTest(ref transComp.Value, Vector3.up, deltaY)) {
                EntityManager.RemoveComponent<FallComponent>(_inputEntity);
            }
            else {
                EntityManager.SetComponentData(_controlEntity, transComp);
            }

            if (Utility.bShowInputLog) {
                Debug.Log("Fall force (" + fallComp.force + ")");
            }

            return true;
        }
        else {
            var transComp = EntityManager.GetComponentData<Translation>(_controlEntity);
            if (false == FallingTest(transComp.Value, Vector3.down, _skinWidth)) {
                EntityManager.AddComponentData(_inputEntity, new FallComponent(0.0f));
            }
        }

        return false;
    }

    private const float _skinWidth = 0.05f;

    private unsafe bool CollisionTest(ref float3 outTrans, float3 inDir, float inDelta) {
        var physWorld = _buildPhysSystem.PhysicsWorld;
        var collider = EntityManager.GetComponentData<PhysicsCollider>(_controlEntity);
        var rotation = EntityManager.GetComponentData<Rotation>(_controlEntity);

        inDir.x *= _speedX;
        inDir.y *= _speedY;
        var delta = inDir * inDelta;
        var newPos = outTrans + (delta);

        var bIsHit = physWorld.CastCollider(new ColliderCastInput {
            Collider = collider.ColliderPtr,
            Orientation = rotation.Value,
            Start = outTrans,
            End = newPos
        }, out var hit);

        if (bIsHit) {
            var offset = hit.Fraction - _skinWidth;
            newPos = math.lerp(outTrans, newPos, offset);
        }

        outTrans = newPos;
        return bIsHit;
    }

    private unsafe bool FallingTest(float3 inTrans, float3 inDir, float inDelta) {
        var physWorld = _buildPhysSystem.PhysicsWorld;
        var collider = EntityManager.GetComponentData<PhysicsCollider>(_controlEntity);
        var rotation = EntityManager.GetComponentData<Rotation>(_controlEntity);

        inDir.x *= _speedX;
        inDir.y *= _speedY;
        var delta = inDir * inDelta;
        var newPos = inTrans + (delta);

        var bIsHit = physWorld.CastCollider(new ColliderCastInput {
            Collider = collider.ColliderPtr,
            Orientation = rotation.Value,
            Start = inTrans,
            End = newPos
        }, out var hit);

        Debug.DrawLine(inTrans, newPos, Color.red);
        return bIsHit;
    }
}