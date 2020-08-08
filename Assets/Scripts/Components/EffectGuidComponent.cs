// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using System;
using Unity.Entities;
using UnityEngine;
using RotaryHeart.Lib.SerializableDictionary;

[Serializable]
public class EffectPresetDataDictionary : SerializableDictionaryBase<EffectUtility.Key, GameObject> {
}

[Serializable]
public struct EffectGuidComponent : IComponentData {
    public EffectUtility.Key id;
    public Entity prefab;
}
