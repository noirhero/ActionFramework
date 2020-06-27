// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using Unity.Entities;
using UnityEngine;
using Unity.Transforms;

public class SpriteChangeSystem : SystemBase {
    protected override void OnUpdate() {
        Entities
            .WithoutBurst()
            .WithStructuralChanges()
            .ForEach((Entity entity, SpriteRenderer renderer, in AnimationFrameComponent state) => {
                var preset = EntityManager.GetSharedComponentData<SpritePresetComponent>(entity);
                if (false == preset.value.datas.TryGetValue(state.currentId, out var animData)) {
                    return;
                }

                var frame = state.frame;
                if (frame > animData.length) {
                    frame = state.bLooping ? frame % animData.length : animData.length;
                }

                var index = 0;
                for (var i = 0; i < animData.timelines.Count; ++i) {
                    var timeline = animData.timelines[i];
                    if (frame >= timeline.start && frame <= timeline.end) {
                        index = i;
                        break;
                    }
                }

                if (EntityManager.HasComponent<AnimationLockComponent>(entity)) {
                    var lockComp = EntityManager.GetComponentData<AnimationLockComponent>(entity);
                    index = index > lockComp.frameIndex ? lockComp.frameIndex : index;
                }

                renderer.sprite = animData.timelines[index].sprite;
                renderer.flipX = state.bFlipX;

                var attackCollision = animData.timelines[index].attackCollision;
                var HasAttackCollision = EntityManager.HasComponent<AttackCollisionComponent>(entity);

                // 현재 프레임에 공격 판정 데이터 있음
                if (0 < attackCollision.width + attackCollision.height) {
                    if (HasAttackCollision) {
                        var attackCollisionComp = EntityManager.GetComponentData<AttackCollisionComponent>(entity);
                        attackCollisionComp.bounds = attackCollision;
                    }
                    else {
                        var newComp = new AttackCollisionComponent() { bounds = attackCollision, pixelsPerUnit = renderer.sprite.pixelsPerUnit };
                        EntityManager.AddComponentData<AttackCollisionComponent>(entity, newComp);
                    }
                }
                // 없으면 삭제
                else {
                    if (HasAttackCollision) {
                        EntityManager.RemoveComponent<AttackCollisionComponent>(entity);
                    }
                }
            })
           .Run();
    }
}