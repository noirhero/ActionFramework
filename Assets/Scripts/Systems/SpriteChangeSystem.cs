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
                    frame %= animData.length;
                }

                foreach (var timeline in animData.timelines) {
                    if (frame >= timeline.start && frame < timeline.end) {
                        renderer.sprite = timeline.sprite;
                        break;
                    }
                }
            })
            .Run();
    }
}