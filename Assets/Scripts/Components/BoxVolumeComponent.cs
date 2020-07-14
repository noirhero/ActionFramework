// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using System;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct BoxVolumeComponent : IComponentData {
    public float2 pos;
    public float2 extends;
}
