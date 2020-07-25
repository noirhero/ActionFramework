// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using System;
using UnityEngine;
using RotaryHeart.Lib.SerializableDictionary;

[Serializable]
public class AudioClipPresetDataDictionary : SerializableDictionaryBase<SoundUtility.ClipKey, AudioClip> {
}

[Serializable]
public class AudioClipPreset : MonoBehaviour {
    [Header("Audio Clip")]
    public AudioClipPresetDataDictionary ClipDatas = new AudioClipPresetDataDictionary();
}
