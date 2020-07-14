// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class AudioSourceProxy : MonoBehaviour, IConvertGameObjectToEntity {
    public SoundUtility.SourceKey id;
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
        var audioSource = GetComponent<AudioSource>();
        if (ReferenceEquals(null, audioSource)) {
            dstManager.DestroyEntity(entity);
            return;
        }

        dstManager.RemoveComponent<LocalToWorld>(entity);
        dstManager.RemoveComponent<Rotation>(entity);
        dstManager.RemoveComponent<Translation>(entity);

        dstManager.AddSharedComponentData(entity, new AudioSourceComponent {
            id = id,
            source = audioSource
        });
    }
}
