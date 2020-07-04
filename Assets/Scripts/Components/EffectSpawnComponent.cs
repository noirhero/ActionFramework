// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using System;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct EffectSpawnComponent : IComponentData {
    public int id;
    public float3 scale;
    public float3 pos;
    public quaternion rot;
}
