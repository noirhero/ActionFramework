// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using Unity.Entities;
using UnityEngine;

public class AudioSystem : SystemBase {
    private AudioClipPreset _audioClipPreset = null;

    protected override void OnStartRunning() {
        Entities
            .WithName("AudioSystem_AudioClip_Collecting")
            .WithoutBurst()
            .ForEach((AudioClipPresetComponent presetComp) => { _audioClipPreset = presetComp.preset; })
            .Run();
    }

    protected override void OnUpdate() {
        Entities
            .WithoutBurst()
            .WithStructuralChanges()
            .ForEach((Entity entity, int entityInQueryIndex, in InstantAudioComponent instantComp) => {
                if (_audioClipPreset.ClipDatas.TryGetValue(instantComp.playID, out var clip)) {
                    AudioSource.PlayClipAtPoint(clip, instantComp.pos);
                    EntityManager.RemoveComponent<InstantAudioComponent>(entity);
                }
            })
            .Run();
    }
}
