// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using System;
using Unity.Entities;

[Serializable]
public struct AudioClipPresetComponent : ISharedComponentData, IEquatable<AudioClipPresetComponent> {
    public AudioClipPreset preset;

    public AudioClipPresetComponent(AudioClipPreset inPreset) {
        preset = inPreset;
    }
    
    public bool Equals(AudioClipPresetComponent other) {
        return ReferenceEquals(other.preset, preset);
    }

    public override int GetHashCode() {
        return ReferenceEquals(null, preset) ? 0 : preset.GetHashCode();
    }
}
