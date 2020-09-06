// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class BGMFadeOutSystem : SystemBase {
    protected override void OnUpdate() {
        var audioSources = new Dictionary<SoundUtility.SourceKey, AudioSource>();
        Entities
            .WithName("BGMFadeOutSystem_Collecting")
            .WithoutBurst()
            .ForEach((in AudioSourceComponent audioSource) => {
                audioSources.Add(audioSource.id, audioSource.source);
            })
            .Run();

        var deltaTime = Time.DeltaTime;
        Entities
            .WithName("BGMFadeOutSystem")
            .WithoutBurst()
            .WithStructuralChanges()
            .ForEach((Entity entity, in BGMFadeOutComponent fadeOut) => {
                if (false == audioSources.TryGetValue(fadeOut.id, out var audioSource)) {
                    return;
                }

                var delta = fadeOut.accum * deltaTime;
                audioSource.volume = math.max(fadeOut.dest, audioSource.volume - delta);
                if (fadeOut.dest >= audioSource.volume) {
                    EntityManager.DestroyEntity(entity);
                }

                if (0.0f >= audioSource.volume && audioSource.isPlaying) {
                    audioSource.Stop();
                }
            })
            .Run();
    }
}