// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using System;
using Unity.Entities;

[Serializable]
public struct AnimationFrameComponent : IComponentData {
    public AnimUtility.AnimKey currentAnim;
    public int state;
    public float frame;
    public float frameRate;
    public bool bFlipX;
    public int currentIndex;
    public bool bFirstChangeIndex;
}

[Serializable]
public struct AnimationLockComponent : IComponentData {
    public int frameIndex;

    public AnimationLockComponent(int inIndex) {
        frameIndex = inIndex;
    }
}
