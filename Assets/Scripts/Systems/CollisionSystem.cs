﻿// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using Unity.Entities;
using UnityEngine;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Physics;

public class CollisionSystem : SystemBase {

    protected override void OnUpdate() {
        Entities
            .WithoutBurst()
            .WithStructuralChanges()
            .ForEach((Entity attacker, in AttackCollisionComponent attackCollisionComp, in PhysicsCollider attackerCollider) => {
                if (attackCollisionComp.bIsConsumed && (false == attackCollisionComp.bShouldMultiCollide))
                    return;

                var attackBounds = attackCollisionComp.bounds;

                // get size
                var entityScale = new Vector2(1.0f, 1.0f);
                if (EntityManager.HasComponent<CompositeScale>(attacker)) {
                    var scaleComp = EntityManager.GetComponentData<CompositeScale>(attacker);
                    entityScale.x = scaleComp.Value.c0.x;
                    entityScale.y = scaleComp.Value.c1.y;
                }
                entityScale *= (1 / attackCollisionComp.pixelsPerUnit);
                var scaledPos = new Vector2(attackBounds.x * entityScale.x, attackBounds.y * entityScale.y);
                var scaledSize = new Vector2(attackBounds.width * entityScale.x, attackBounds.height * entityScale.y);

                // get position
                var attackerTranslation = EntityManager.GetComponentData<Translation>(attacker);
                scaledPos += new Vector2(attackerTranslation.Value.x - (scaledSize.x * 0.5f), attackerTranslation.Value.y);

                var attackCollision = new Rect(scaledPos, scaledSize);

#if UNITY_EDITOR
                Debug.DrawLine(new Vector3(attackCollision.xMin, attackCollision.yMin), new Vector3(attackCollision.xMax, attackCollision.yMin), Color.red);   // top
                Debug.DrawLine(new Vector3(attackCollision.xMin, attackCollision.yMax), new Vector3(attackCollision.xMax, attackCollision.yMax), Color.red);   // bottom
                Debug.DrawLine(new Vector3(attackCollision.xMin, attackCollision.yMin), new Vector3(attackCollision.xMin, attackCollision.yMax), Color.red);   // left
                Debug.DrawLine(new Vector3(attackCollision.xMax, attackCollision.yMin), new Vector3(attackCollision.xMax, attackCollision.yMax), Color.red);   // right
#endif

                var attackerId = EntityManager.GetComponentData<TargetIdComponent>(attacker).value;
                var localAttackCollisionComp = attackCollisionComp;
                var attackColliderFilter = attackerCollider.Value.Value.Filter.BelongsTo;
                Entities.WithoutBurst().WithStructuralChanges().WithNone<HitComponent>().ForEach((Entity hitTarget, in PhysicsCollider hitTargetCollider) => {
                    if (attacker == hitTarget)
                        return;

                    // 현재 같은 Controller 필터일 경우에만 충돌 감지 처리
                    if (hitTargetCollider.Value.Value.Filter.BelongsTo != attackColliderFilter)
                        return;

                    var targetId = EntityManager.GetComponentData<TargetIdComponent>(hitTarget).value;
                    if (attackerId == targetId) {
                        return;
                    }

                    var targetTranslation = EntityManager.GetComponentData<Translation>(hitTarget);

                    var distance = math.length(attackerTranslation.Value - targetTranslation.Value);
                    if (1.0f > distance) {
                        var targetAabb = hitTargetCollider.Value.Value.CalculateAabb();

                        var targetSize = new Vector2(targetAabb.Extents.x, targetAabb.Extents.y);
                        var targetPosition = new Vector2(targetTranslation.Value.x + targetAabb.Center.x - (targetSize.x * 0.5f), 
                                                         targetTranslation.Value.y + targetAabb.Center.y - (targetSize.y * 0.5f));

                        var targetCollision = new Rect(targetPosition, targetSize);

                        // 충돌 감지!
                        if (attackCollision.Overlaps(targetCollision)) {
                            if (EntityManager.HasComponent<TargetIdComponent>(hitTarget)) {
                                if (IdUtility.Id.Player == targetId) {
                                    if (false == GameOver.bIsOvered) {
                                        EntityManager.AddComponentData(hitTarget, new InstantAudioComponent() {
                                            id = SoundUtility.ClipKey.Damage,
                                            pos = targetTranslation.Value,
                                        });
                                    }
                                    GameOver.Over();
                                    return;
                                }
                            }

                            localAttackCollisionComp.bIsConsumed = true;
                            EntityManager.SetComponentData(attacker, localAttackCollisionComp);

                            var renderer = EntityManager.GetComponentObject<SpriteRenderer>(attacker);
                            var dir = renderer.flipX ? -1.0f : 1.0f;
                            var moveComponent = new MoveComponent() {
                                impulseDir = new Vector2(dir, 0.0f),
                                impulseForce = 1.0f
                            };
                            EntityManager.AddComponentData(hitTarget, moveComponent);

                            var hitComponent = new HitComponent() {
                                damage = 10,
                                godTime = 0.5f,
                                hitEffectColor = Color.black,
                                hitEffectTime = 0.1f
                            };
                            EntityManager.AddComponentData(hitTarget, hitComponent);
                            EntityManager.AddComponentData(hitTarget, new InstantAudioComponent() {
                                id = SoundUtility.ClipKey.Hit,
                                pos = targetTranslation.Value,
                            });

                            var effectSpawnEntity = EntityManager.CreateEntity();
                            EntityManager.AddComponentData(effectSpawnEntity, new EffectSpawnComponent {
                                id = EffectUtility.Key.Hit,
                                pos = targetTranslation.Value,
                                rot = quaternion.identity,
                                scale = new float3(1.0f)
                            });
                        }

#if UNITY_EDITOR
                        Debug.DrawLine(new Vector3(targetCollision.xMin, targetCollision.yMin), new Vector3(targetCollision.xMax, targetCollision.yMin), Color.red);   // top
                        Debug.DrawLine(new Vector3(targetCollision.xMax, targetCollision.yMin), new Vector3(targetCollision.xMax, targetCollision.yMax), Color.red);   // right
                        Debug.DrawLine(new Vector3(targetCollision.xMin, targetCollision.yMax), new Vector3(targetCollision.xMax, targetCollision.yMax), Color.red);   // bottom
                        Debug.DrawLine(new Vector3(targetCollision.xMin, targetCollision.yMin), new Vector3(targetCollision.xMin, targetCollision.yMax), Color.red);   // left
#endif
                    }
                }).Run();

            }).Run();
    }
}
