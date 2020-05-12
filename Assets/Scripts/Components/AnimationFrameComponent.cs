// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using System;
using Unity.Entities;

[Serializable]
public struct AnimationFrameComponent : IComponentData {
    public int setId;
    public int currentId;
    public float frame;
}
