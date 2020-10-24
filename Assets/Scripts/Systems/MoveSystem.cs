// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(InputSystemEX))]
public class MoveSystem : ComponentSystem {
    private BuildPhysicsWorld _buildPhysSystem;
    private bool _hasBeenInitialized = false;

    protected override void OnStartRunning() {
        if (_hasBeenInitialized) {
            return;
        }

        _hasBeenInitialized = true;
        _buildPhysSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();
    }

    protected override void OnUpdate() {
        Entities.ForEach((Entity entity, ref Translation transComp, ref MoveComponent moveComp,
            ref AnimationFrameComponent animComp) => {
            var calcPos = transComp.Value;
            calcPos.y = GetPositionY(entity, calcPos);
            calcPos.x = GetPositionX(entity, calcPos);
            var dir = calcPos - transComp.Value;

            if (false == EntityManager.HasComponent<TargetIdFollowComponent>(entity)) {
#region Run

                var bMovingX = math.FLT_MIN_NORMAL < math.abs(moveComp.value.x);
                var bRunning = ((0.0f < dir.x) && (Utility.stepOffset < dir.x)) ||
                               (0.0f > dir.x) && (-Utility.stepOffset > dir.x);
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

#endregion

#region Jump

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
                        EntityManager.AddComponentData(entity, new InstantAudioComponent() {
                            id = SoundUtility.ClipKey.Landing,
                            pos = calcPos,
                        });
                        var effectSpawnEntity = EntityManager.CreateEntity();
                        EntityManager.AddComponentData(effectSpawnEntity, new EffectSpawnComponent {
                            id = EffectUtility.Key.Landing,
                            pos = calcPos,
                            rot = quaternion.identity,
                            scale = new float3(0.5f)
                        });
                    }
                }

#endregion

#region Crouch

                if (EntityManager.HasComponent<CrouchComponent>(entity)) {
                    animComp.state |= AnimUtility.crouch;
                }
                else if (AnimUtility.HasState(animComp, AnimUtility.crouch)) {
                    animComp.state ^= AnimUtility.crouch;
                }

#endregion

#region FrameLock

                // jump anim lock
                if (EntityManager.HasComponent<CrouchComponent>(entity)) {
                    EntityManager.AddComponentData(entity, new AnimationLockComponent(1));
                }
                else if (bJumping) {
                    if (false == EntityManager.HasComponent<AnimationLockComponent>(entity)) {
                        EntityManager.AddComponentData(entity, new AnimationLockComponent(2));
                    }
                }
                else {
                    if (EntityManager.HasComponent<AnimationLockComponent>(entity)) {
                        EntityManager.RemoveComponent<AnimationLockComponent>(entity);
                    }
                }

#endregion
            }

            transComp.Value = calcPos;
        });
    }

    private float GetPositionX(Entity entity, float3 inPos) {
        var moveComp = EntityManager.GetComponentData<MoveComponent>(entity);
        var moveValue = moveComp.value;

        var velocity = float3.zero;
        velocity.x = moveValue.x * Utility.speedX * Time.fixedDeltaTime;

        // add impulse
        if (0.0f < moveComp.impulseForce) {
            velocity.x += moveComp.impulseForce * moveComp.impulseDir.x * Time.fixedDeltaTime;
        }

        if (EntityManager.HasComponent<TargetIdComponent>(entity)) {
            var targetId = EntityManager.GetComponentData<TargetIdComponent>(entity).value;
            if (IdUtility.Id.Enemy == targetId) {
                inPos += velocity;
            }
            else {
                CollisionTest(entity, velocity, ref inPos);
            }
        }
        return inPos.x;
    }

    private float GetPositionY(Entity entity, float3 inPos) {
        var moveValue = EntityManager.GetComponentData<MoveComponent>(entity).value;
        if (math.FLT_MIN_NORMAL >= math.abs(moveValue.y)) {
            return inPos.y;
        }

        var velocity = float3.zero;
        velocity.y = moveValue.y * Utility.speedY * Time.fixedDeltaTime;

        CollisionTest(entity, velocity, ref inPos);
        return inPos.y;
    }

    private unsafe void CollisionTest(Entity entity, float3 inVelocity, ref float3 outPos) {
        var isVelocityNaN = math.isnan(math.normalize(inVelocity));
        if (isVelocityNaN.x | isVelocityNaN.y | isVelocityNaN.z) {
            return;
        }

        var physWorld = _buildPhysSystem.PhysicsWorld;
        var collider = EntityManager.GetComponentData<PhysicsCollider>(entity);
        var rotation = EntityManager.GetComponentData<Rotation>(entity);

        var startPos = outPos;
        var skinWidth = math.normalize(inVelocity) * Utility.skinWidth;
        var newPos = outPos + inVelocity + skinWidth;

        var bIsHit = physWorld.CastCollider(new ColliderCastInput {
            Collider = collider.ColliderPtr,
            Orientation = rotation.Value,
            Start = startPos,
            End = newPos
        }, out var hit);

        if (bIsHit) {
            newPos = math.lerp(startPos, newPos, hit.Fraction);
        }

        newPos -= skinWidth;
        outPos = newPos;
    }
}
