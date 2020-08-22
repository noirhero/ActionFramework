// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class BGMFadeInSystem : SystemBase {
    protected override void OnUpdate() {
        var audioSources = new Dictionary<SoundUtility.SourceKey, AudioSource>();
        Entities
            .WithName("BGMFadeInSystem_Collecting")
            .WithoutBurst()
            .ForEach((in AudioSourceComponent audioSource) => {
                audioSources.Add(audioSource.id, audioSource.source);
            })
            .Run();

        var deltaTime = Time.DeltaTime;
        Entities
            .WithName("BGMFadeInSystem")
            .WithoutBurst()
            .WithStructuralChanges()
            .ForEach((Entity entity, in AudioSourceControlComponent audioSourceControl) => {
                if (false == audioSources.TryGetValue(audioSourceControl.id, out var audioSource)) {
                    return;
                }

                var delta = audioSourceControl.accume * deltaTime;
                audioSource.volume = math.min(audioSourceControl.maxVolume, audioSource.volume + delta);
                if (audioSourceControl.maxVolume <= audioSource.volume) {
                    EntityManager.DestroyEntity(entity);
                }
            })
            .Run();
    }
}
