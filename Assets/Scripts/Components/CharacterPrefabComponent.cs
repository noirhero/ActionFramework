// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using System;
using Unity.Entities;
using UnityEngine;
using RotaryHeart.Lib.SerializableDictionary;

[Serializable]
public class CharacterPrefabDictionary : SerializableDictionaryBase<IdUtility.Id, GameObject> {
}

[Serializable]
public struct CharacterPrefabComponent : IComponentData {
    public IdUtility.Id id;
    public Entity prefab;
}
