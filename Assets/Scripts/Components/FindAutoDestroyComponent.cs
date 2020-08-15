// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using System;
using Unity.Entities;

[Serializable]
[GenerateAuthoringComponent]
public struct FindAutoDestroyComponent : IComponentData {
    public int loopingCount;
}
