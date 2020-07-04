// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using System;
using Unity.Entities;

[Serializable]
public struct EffectGuidComponent : IComponentData {
    public int id;
    public Entity prefab;
}
