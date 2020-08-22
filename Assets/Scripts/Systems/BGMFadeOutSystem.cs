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
            .ForEach((Entity entity, in AudioSourceControlComponent audioSourceControl) => {
                if (false == audioSources.TryGetValue(audioSourceControl.id, out var audioSource)) {
                    return;
                }

                var delta = audioSourceControl.accume * deltaTime;
                audioSource.volume = math.max(audioSourceControl.minVolume, audioSource.volume - delta);
                if (audioSourceControl.minVolume >= audioSource.volume) {
                    EntityManager.DestroyEntity(entity);
                }
            })
            .Run();
    }
}