// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using Unity.Entities;

public class AnimationFrameSystem : SystemBase {
    protected override void OnUpdate() {
        var deltaTime = Time.DeltaTime;

        Entities
            .WithBurst()
            .ForEach((ref AnimationFrameComponent animComp) => {
                var animKey = AnimUtility.GetAnimKey(animComp);

                if (animComp.currentAnim != animKey) {
                    animComp.currentAnim = animKey;
                    animComp.frame = 0.0f;
                    animComp.frameRate = 0.0f;
                    return;
                }

                animComp.frame += deltaTime;
            })
            .ScheduleParallel(Dependency)
            .Complete();
    }
}
