// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using System;
using UnityEngine;
using RotaryHeart.Lib.SerializableDictionary;

[Serializable]
public struct AudioClipPresetData {
    public AudioClip clip;
}

[Serializable]
public class AudioClipPresetDataDictionary : SerializableDictionaryBase<SoundUtility.SourceKey, AudioClipPresetData> {
}

[Serializable]
public class AudioClipPreset : MonoBehaviour {
    [Header("Audio Clip")]
    public AudioClipPresetDataDictionary ClipDatas = new AudioClipPresetDataDictionary();
}
