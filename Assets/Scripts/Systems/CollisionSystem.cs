// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using Unity.Entities;
using UnityEngine;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Physics;
using UnityEditor;

public class CollisionSystem : SystemBase {

    protected override unsafe void OnUpdate() {
        Entities
            .WithoutBurst()
            .WithStructuralChanges()
            .ForEach((Entity attacker, AttackCollisionComponent attackCollisionComp, PhysicsCollider attackerCollider) => {

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

                { // preview
                    Debug.DrawLine(new Vector3(attackCollision.xMin, attackCollision.yMin), new Vector3(attackCollision.xMax, attackCollision.yMin));   // top
                    Debug.DrawLine(new Vector3(attackCollision.xMin, attackCollision.yMax), new Vector3(attackCollision.xMax, attackCollision.yMax));   // bottom
                    Debug.DrawLine(new Vector3(attackCollision.xMin, attackCollision.yMin), new Vector3(attackCollision.xMin, attackCollision.yMax));   // left
                    Debug.DrawLine(new Vector3(attackCollision.xMax, attackCollision.yMin), new Vector3(attackCollision.xMax, attackCollision.yMax));   // right
                }

                Entities.WithoutBurst().WithStructuralChanges().ForEach((Entity hitTarget, PhysicsCollider hitTargetCollider) => {
                    if (attacker == hitTarget)
                        return;

                    // 현재 같은 Controller 필터일 경우에만 충돌 감지 처리
                    if (hitTargetCollider.Value.Value.Filter.BelongsTo != attackerCollider.Value.Value.Filter.BelongsTo)
                        return;

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
                            Debug.Log("Overlaps! '-^");

                            var effectSpawnEntity = EntityManager.CreateEntity();
                            EntityManager.AddComponentData(effectSpawnEntity, new EffectSpawnComponent {
                                pos = targetTranslation.Value,
                                rot = quaternion.identity,
                                scale = new float3(2.0f)
                            });
                        }

                        { // preview
                            Debug.DrawLine(new Vector3(targetCollision.xMin, targetCollision.yMin), new Vector3(targetCollision.xMax, targetCollision.yMin));   // top
                            Debug.DrawLine(new Vector3(targetCollision.xMin, targetCollision.yMax), new Vector3(targetCollision.xMax, targetCollision.yMax));   // bottom
                            Debug.DrawLine(new Vector3(targetCollision.xMin, targetCollision.yMin), new Vector3(targetCollision.xMin, targetCollision.yMax));   // left
                            Debug.DrawLine(new Vector3(targetCollision.xMax, targetCollision.yMin), new Vector3(targetCollision.xMax, targetCollision.yMax));   // right
                        }
                    }
                }).Run();

            }).Run();
    }
}