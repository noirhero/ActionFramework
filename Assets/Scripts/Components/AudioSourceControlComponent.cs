// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using System;
using Unity.Entities;

[Serializable]
public struct AudioSourceControlComponent : IComponentData {
    public SoundUtility.SourceKey id;
    public float accume;
    public float minVolume;
    public float maxVolume;
    public float currentVolume;
    public float oldVolume;
}
