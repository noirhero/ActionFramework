// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using Unity.Entities;
using Unity.Jobs;

public class AnimationFrameSystem : JobComponentSystem {
    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        var deltaTime = Time.DeltaTime;
        return Entities
            .WithBurst()
            .ForEach((ref AnimationFrameComponent state) => {
                if (state.setId != state.currentId) {
                    state.currentId = state.setId;
                    state.frame = 0.0f;
                    state.bDone = false;
                }

                // default setting
                if (state.bDone) {
                    state.setId = Utility.AnimState.Idle;
                }

                state.frame += deltaTime;
            })
            .Schedule(inputDeps);
    }
}
