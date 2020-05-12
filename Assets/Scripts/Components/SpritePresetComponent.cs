// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using System;
using Unity.Entities;

[Serializable]
public struct SpritePresetComponent : ISharedComponentData, IEquatable<SpritePresetComponent> {
    public SpritePreset value;

    public bool Equals(SpritePresetComponent other) {
        return ReferenceEquals(other.value, value);
    }

    public override int GetHashCode() {
        return ReferenceEquals(null, value) ? 0 : value.GetHashCode();
    }
}
