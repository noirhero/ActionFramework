// Copyright 2018-202 TAP, Inc. All Rights Reserved.

using System;
using System.Collections.Generic;
using UnityEngine;
using RotaryHeart.Lib.SerializableDictionary;

[Serializable]
public class AnimTimeline {
    public float start;
    public float end;
    public Sprite sprite;
    public Rect attackCollision;
    public bool bUseMultiCollision;
    public SoundUtility.ClipKey soundClipKey;
}

[Serializable]
public class AnimData {
    public float length;
    public List<AnimTimeline> timelines;
}

[Serializable]
public class AnimDatas : SerializableDictionaryBase<AnimUtility.AnimKey, AnimData> {
}

public class SpritePreset : MonoBehaviour {
    public List<AnimationClip> clips = new List<AnimationClip>();
    public AnimDatas datas = new AnimDatas();
}
