// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using Unity.Entities;
using UnityEngine;

public class SpriteChangeSystem : SystemBase {
    protected override void OnUpdate() {
        Entities.WithoutBurst()
                .WithStructuralChanges()
                .ForEach((Entity entity, SpriteRenderer renderer, ref AnimationFrameComponent animComp) => {
                    if (false == EntityManager.HasComponent<SpritePresetComponent>(entity)) {
                        return;
                    }

                    var preset = EntityManager.GetSharedComponentData<SpritePresetComponent>(entity);
                    if (false == preset.value.datas.TryGetValue(animComp.currentAnim, out var animData)) {
                        return;
                    }

                    var frame = animComp.frame;
                    if (frame > animData.length) {
                        frame = AnimUtility.IsLooping(animComp) ? frame % animData.length : animData.length;
                    }

                    animComp.frameRate = frame / animData.length;

                    var index = 0;
                    for (var i = 0; i < animData.timelines.Count; ++i) {
                        var timeline = animData.timelines[i];

                        if (frame >= timeline.start &&
                            frame <= timeline.end) {
                            index = i;

                            break;
                        }
                    }

                    if (EntityManager.HasComponent<AnimationLockComponent>(entity)) {
                        var lockComp = EntityManager.GetComponentData<AnimationLockComponent>(entity);
                        index = index > lockComp.frameIndex ? lockComp.frameIndex : index;
                    }

                    renderer.sprite = animData.timelines[index]
                                              .sprite;

                    renderer.flipX = animComp.bFlipX;

                    var attackCollision = animData.timelines[index]
                                                  .attackCollision;

                    var HasAttackCollisionComp = EntityManager.HasComponent<AttackCollisionComponent>(entity);

                    if (renderer.flipX) {
                        attackCollision.x *= -1.0f;
                    }

                    // 현재 프레임에 공격 판정 데이터 있음
                    if ((0 < attackCollision.width) && (0 < attackCollision.height)) {
                        if (HasAttackCollisionComp) {
                            var attackCollisionComp = EntityManager.GetComponentData<AttackCollisionComponent>(entity);

                            // 이전 프레임의 값이 남은 경우
                            if (index != attackCollisionComp.animationFrame) {
                                attackCollisionComp.bounds = attackCollision;
                                attackCollisionComp.animationFrame = index;
                                attackCollisionComp.bIsConsumed = false;

                                EntityManager.SetComponentData<AttackCollisionComponent>(entity, attackCollisionComp);
                            }
                        }
                        else {
                            var newComp = new AttackCollisionComponent() {
                                bounds = attackCollision,
                                pixelsPerUnit = renderer.sprite.pixelsPerUnit,
                                animationFrame = index,
                                bShouldMultiCollide = animData.timelines[index].bUseMultiCollision
                            };

                            EntityManager.AddComponentData<AttackCollisionComponent>(entity, newComp);
                        }
                    }
                    // 이전 프레임에는 있었으나 지금은 없음
                    else if (HasAttackCollisionComp) {
                        EntityManager.RemoveComponent<AttackCollisionComponent>(entity);
                    }
                })
                .Run();
    }
}
