// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class CharacterPrefabProxy : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity {
    public CharacterPrefabDictionary prefabs = new CharacterPrefabDictionary();

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs) {
        foreach (var prefabPair in prefabs) {
            referencedPrefabs.Add(prefabPair.Value);
        }
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
        foreach (var prefabPair in prefabs) {
            dstManager.AddComponentData(dstManager.CreateEntity(), new CharacterPrefabComponent {
                id = prefabPair.Key,
                prefab = conversionSystem.GetPrimaryEntity(prefabPair.Value)
            });
        }
        dstManager.DestroyEntity(entity);
    }
}
