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

                animComp.currentIndex = index;

                if (EntityManager.HasComponent<AnimationLockComponent>(entity)) {
                    var lockComp = EntityManager.GetComponentData<AnimationLockComponent>(entity);
                    index = index > lockComp.frameIndex ? lockComp.frameIndex : index;
                }

                renderer.sprite = animData.timelines[index].sprite;
                renderer.flipX = animComp.bFlipX;
            })
            .Run();
    }
}
