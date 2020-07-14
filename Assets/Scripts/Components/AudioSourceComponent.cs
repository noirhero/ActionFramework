// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using System;
using Unity.Entities;
using UnityEngine;

[Serializable]
public struct AudioSourceComponent : ISharedComponentData, IEquatable<AudioSourceComponent> {
    public SoundUtility.SourceKey id;
    public AudioSource source;

    public bool Equals(AudioSourceComponent other) {
        return ReferenceEquals(other.source, source);
    }

    public override int GetHashCode() {
        return ReferenceEquals(null, source) ? 0 : source.GetHashCode();
    }
}
