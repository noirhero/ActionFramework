// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using System;
using Unity.Entities;

[Serializable]
[GenerateAuthoringComponent]
public struct BGMFadeOutComponent : IComponentData {
    public SoundUtility.SourceKey id;
    public float accum;
    public float dest;
}
