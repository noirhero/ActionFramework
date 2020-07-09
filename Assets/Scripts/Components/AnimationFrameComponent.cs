﻿// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using System;
using Unity.Entities;

[Serializable]
public struct AnimationFrameComponent : IComponentData {
    public AnimState.AnimKey currentAnim;
    public int state;
    public float frame;
    public bool bFlipX;
}

[Serializable]
public struct AnimationLockComponent : IComponentData {
    public int frameIndex;

    public AnimationLockComponent(int inIndex) {
        frameIndex = inIndex;
    }
}
