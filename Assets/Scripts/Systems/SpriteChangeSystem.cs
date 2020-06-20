// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using Unity.Entities;
using UnityEngine;
using Unity.Transforms;

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

                // collision preview
                var entityScale = new Vector2(1.0f, 1.0f);
                if (EntityManager.HasComponent<CompositeScale>(entity))
                {
                   var scaleComp = EntityManager.GetComponentData<CompositeScale>(entity);
                   entityScale.x = scaleComp.Value.c0.x;
                   entityScale.y = scaleComp.Value.c1.y;
                }
                entityScale *= (1 / renderer.sprite.pixelsPerUnit);

                var transComp = EntityManager.GetComponentData<Translation>(entity);

                var attackCollision = animData.timelines[index].attackCollision;
                var collision = new Rect(transComp.Value.x, transComp.Value.y, 
                                         attackCollision.width * entityScale.x, 
                                         attackCollision.height * entityScale.y);
             
                var left_x = collision.x - (collision.width * 0.5f);
                var right_x = collision.x + (collision.width * 0.5f);
                var top_y = collision.y;
                var bottom_y = collision.y + collision.height;
                Debug.DrawLine(new Vector3(left_x, top_y), new Vector3(right_x, top_y));
                Debug.DrawLine(new Vector3(right_x, top_y), new Vector3(right_x, bottom_y));
                Debug.DrawLine(new Vector3(right_x, bottom_y), new Vector3(left_x, bottom_y));
                Debug.DrawLine(new Vector3(left_x, bottom_y), new Vector3(left_x, top_y));
           })
           .Run();
    }
}