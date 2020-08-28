// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using System;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
[GenerateAuthoringComponent]
public struct CharacterLoadComponent : IComponentData {
    public IdUtility.Id id;
    public float3 pos;
}
