﻿// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using Unity.Entities;
using Unity.Jobs;

public class AnimationFrameSystem : JobComponentSystem {
    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        var deltaTime = Time.DeltaTime;
        return Entities
              .WithBurst()
              .ForEach((ref AnimationFrameComponent animComp) => {
                   var animID = AnimState.GetAnimID(animComp);
                   if (animComp.currentAnim != animID) {
                       animComp.currentAnim = animID;
                       animComp.frame = 0.0f;
                   }
                   else {
                       animComp.frame += deltaTime;
                   }
               })
              .Schedule(inputDeps);
    }
}
