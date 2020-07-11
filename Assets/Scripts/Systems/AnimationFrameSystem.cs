// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

public class AnimationFrameSystem : JobComponentSystem {
    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        var deltaTime = Time.DeltaTime;

        return Entities.WithBurst()
            .ForEach((ref AnimationFrameComponent animComp) => {
                var animKey = AnimUtility.GetAnimKey(animComp);

                if (animComp.currentAnim != animKey) {
                    animComp.currentAnim = animKey;
                    animComp.frame = 0.0f;
                    animComp.frameRate = 0.0f;
                }
                else {
                    animComp.frame += deltaTime;
                }
            })
            .Schedule(inputDeps);
    }
}
