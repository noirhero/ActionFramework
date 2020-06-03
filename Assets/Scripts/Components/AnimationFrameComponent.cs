// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using System;
using Unity.Entities;

[Serializable]
public struct AnimationFrameComponent : IComponentData {
    public Utility.AnimState setId;
    public Utility.AnimState currentId;
    public float frame;
    public bool bFlipX;
    public bool bLooping;
    public int lockFrameIndex;
}
