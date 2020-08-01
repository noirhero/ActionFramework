﻿// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

public class MoveSystem : ComponentSystem {
    private Entity _inputEntity;
    private Entity _controlEntity;
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

        var currentPos = EntityManager.GetComponentData<Translation>(_controlEntity).Value;
        var calcPos = currentPos;
        calcPos.y = GetPositionY(calcPos);
        calcPos.x = GetPositionX(calcPos);
        var dir = calcPos - currentPos;

        var moveComp = EntityManager.GetComponentData<MoveComponent>(_controlEntity);
        var animComp = EntityManager.GetComponentData<AnimationFrameComponent>(_controlEntity);

        // run
        var bMovingX = math.FLT_MIN_NORMAL < math.abs(moveComp.value.x);
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
        var bMovingY = math.FLT_MIN_NORMAL < math.abs(Utility.terminalVelocity - moveComp.value.y);
        var bFalling = (0.0f > dir.y) && (-Utility.stepOffset > dir.y);
        var bJumping = (0.0f < dir.y) && (Utility.stepOffset < dir.y);
        if (bMovingY || bJumping || bFalling) {
            if (false == AnimUtility.HasState(animComp, AnimUtility.jump)) {
                animComp.state |= AnimUtility.jump;
            }
        }
        else {
            if (AnimUtility.HasState(animComp, AnimUtility.jump)) {
                animComp.state ^= AnimUtility.jump;
                EntityManager.AddComponentData(_controlEntity, new InstantAudioComponent() {
                    playID = SoundUtility.ClipKey.FootStep,
                    pos = calcPos,
                });
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
        EntityManager.SetComponentData(_controlEntity, new Translation {
            Value = calcPos
        });
    }

    private float GetPositionX(float3 inPos) {
        var moveValue = EntityManager.GetComponentData<MoveComponent>(_controlEntity).value;
        if (math.FLT_MIN_NORMAL >= math.abs(moveValue.x)) {
            return inPos.x;
        }

        var velocity = float3.zero;
        velocity.x = moveValue.x * Utility.speedX * Time.fixedDeltaTime;

        CollisionTest(velocity, ref inPos);
        return inPos.x;
    }

    private float GetPositionY(float3 inPos) {
        var moveValue = EntityManager.GetComponentData<MoveComponent>(_controlEntity).value;
        if (math.FLT_MIN_NORMAL >= math.abs(moveValue.y)) {
            return inPos.y;
        }

        var velocity = float3.zero;
        velocity.y = moveValue.y * Utility.speedY * Time.fixedDeltaTime;

        CollisionTest(velocity, ref inPos);
        return inPos.y;
    }

    private unsafe void CollisionTest(float3 inVelocity, ref float3 outPos) {
        var physWorld = _buildPhysSystem.PhysicsWorld;
        var collider = EntityManager.GetComponentData<PhysicsCollider>(_controlEntity);
        var rotation = EntityManager.GetComponentData<Rotation>(_controlEntity);

        var startPos = outPos;
        var newPos = outPos + inVelocity;

        var skinWidth = math.normalize(inVelocity) * Utility.skinWidth;
        var bIsHit = physWorld.CastCollider(new ColliderCastInput {
            Collider = collider.ColliderPtr,
            Orientation = rotation.Value,
            Start = startPos,
            End = newPos + skinWidth
        }, out var hit);

        if (bIsHit) {
            newPos = math.lerp(startPos, newPos, hit.Fraction);
        }
        newPos -= skinWidth;

        outPos = newPos;
    }
}
