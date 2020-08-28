// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class EffectPoolProxy : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity {
    public EffectPresetDataDictionary effectObjects = new EffectPresetDataDictionary();

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs) {
        foreach (var effectObject in effectObjects) {
            referencedPrefabs.Add(effectObject.Value);
        }
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
        foreach (var effectObject in effectObjects) {
            dstManager.AddComponentData(dstManager.CreateEntity(), new EffectGuidComponent {
                id = effectObject.Key,
                prefab =  conversionSystem.GetPrimaryEntity(effectObject.Value)
            });
        }
        dstManager.DestroyEntity(entity);
    }
}
