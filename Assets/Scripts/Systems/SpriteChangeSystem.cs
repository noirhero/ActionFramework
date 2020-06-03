// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using Unity.Entities;
using UnityEngine;

public class SpriteChangeSystem : SystemBase {
    protected override void OnUpdate() {
        Entities
            .WithoutBurst()
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
                for (int i = 0; i < animData.timelines.Count; ++i) {
                    var timeline = animData.timelines[i];
                    if (frame >= timeline.start && frame <= timeline.end) {
                        index = i;
                        break;
                    }
                }

                var bIsLockFrame = state.lockFrameIndex != Utility.INDEX_NONE;
                if (bIsLockFrame) {
                    index = index > state.lockFrameIndex ? state.lockFrameIndex : index;
                }
                
                renderer.sprite = animData.timelines[index].sprite;
                renderer.flipX = state.bFlipX;
            })
            .Run();
    }
}