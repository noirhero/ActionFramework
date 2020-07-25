// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class BGMVolumeSystem : SystemBase {
    private Transform _cameraTransform;
    private AudioClipPreset _audioClipPreset = null;

    protected override void OnStartRunning() {
        if (Camera.main != null) {
            _cameraTransform = Camera.main.transform;
        }

        Entities
            .WithName("BGMVolumeSystem_AudioClip_Collecting")
            .WithoutBurst()
            .ForEach((in AudioClipPresetComponent presetComp) => {
                _audioClipPreset = presetComp.preset;
            })
            .Run();
    }

    protected override void OnUpdate() {
        var audioSources = new Dictionary<SoundUtility.SourceKey, AudioSource>();
        Entities
            .WithName("BGMVolumeSystem_AudioSource_Collecting")
            .WithoutBurst()
            .ForEach((in AudioSourceComponent source) => {
                audioSources.Add(source.id, source.source);
            })
            .Run();

        var cameraPos = _cameraTransform.position;
        var deltaTime = Time.DeltaTime;
        Entities
            .WithName("BGMVolumeSystem_Collision")
            .WithoutBurst()
            .WithStructuralChanges()
            .ForEach((ref AudioSourceControlComponent control, in BoxVolumeComponent volume) => {
                var isInX = volume.extends.x >= math.abs(volume.pos.x - cameraPos.x);
                var isInY = volume.extends.y >= math.abs(volume.pos.y - cameraPos.y);
                var delta = (isInX && isInY ? control.accume : -control.accume) * deltaTime;
                var calcVolume = math.clamp(control.currentVolume + delta, control.minVolume, control.maxVolume);

                if (math.FLT_MIN_NORMAL >= math.abs(control.oldVolume - calcVolume)) {
                    return;
                }

                if (false == audioSources.TryGetValue(control.id, out var source)) {
                    return;
                }

                if (_audioClipPreset.ClipDatas.TryGetValue(control.clipID, out var clip)) {
                    source.clip = clip;
                }

                control.oldVolume = control.currentVolume = calcVolume;
                source.volume = control.currentVolume;

                if (math.FLT_MIN_NORMAL > calcVolume && source.isPlaying) {
                    source.Pause();
                }
                else if (math.FLT_MIN_NORMAL < calcVolume && false == source.isPlaying) {
                    source.Play();
                }
            })
            .Run();
    }
}
