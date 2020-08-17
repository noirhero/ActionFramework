// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using System;
using Unity.Entities;

[Serializable]
[GenerateAuthoringComponent]
public struct InputDataComponent : IComponentData {
    public int state;
    public float dir;
}
