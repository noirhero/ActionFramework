// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class BGMVolumeProxy : MonoBehaviour, IConvertGameObjectToEntity {
    public SoundUtility.SourceKey id;
    public SoundUtility.ClipKey clipID;
    public float accume = 0.2f;
    public float minVolume = 0.0f;
    public float maxVolume = 1.0f;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
        var collider = GetComponent<BoxCollider2D>();
        if (ReferenceEquals(null, collider)) {
            dstManager.DestroyEntity(entity);
            return;
        }

        dstManager.RemoveComponent<LocalToWorld>(entity);
        dstManager.RemoveComponent<Rotation>(entity);
        dstManager.RemoveComponent<Translation>(entity);

        var pos = GetComponent<Transform>().position;
        var offset = collider.offset;
        var size = collider.size;
        dstManager.AddComponentData(entity, new BoxVolumeComponent {
            pos = new float2(pos.x + offset.x, pos.y + offset.y),
            extends = new float2(size.x * 0.5f, size.y * 0.5f)
        });

        dstManager.AddComponentData(entity, new AudioSourceControlComponent {
            id = id,
            clipID = clipID,
            accume = accume,
            minVolume = minVolume,
            maxVolume = maxVolume
        });
    }
}
