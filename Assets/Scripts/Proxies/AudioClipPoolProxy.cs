// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class AudioClipPoolProxy : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity {
    public AudioClipPreset _preset;

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs) {
        referencedPrefabs.Add(_preset.gameObject);
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
        
        dstManager.RemoveComponent<LocalToWorld>(entity);
        dstManager.RemoveComponent<Rotation>(entity);
        dstManager.RemoveComponent<Translation>(entity);
        
        dstManager.AddSharedComponentData(entity, new AudioClipPresetComponent(_preset));
    }
}
