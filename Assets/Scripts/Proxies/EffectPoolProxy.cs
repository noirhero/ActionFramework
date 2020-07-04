// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class EffectPoolProxy : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity {
    public List<GameObject> effectObjects = new List<GameObject>();

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs) {
        foreach (var effectObject in effectObjects) {
            referencedPrefabs.Add(effectObject);
        }
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
        for (var i = 0; i < effectObjects.Count; ++i) {
            dstManager.AddComponentData(dstManager.CreateEntity(), new EffectGuidComponent {
                id = i,
                prefab =  conversionSystem.GetPrimaryEntity(effectObjects[i])
            });
        }
        dstManager.DestroyEntity(entity);
    }
}
