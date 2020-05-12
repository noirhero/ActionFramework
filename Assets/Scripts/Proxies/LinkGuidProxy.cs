// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class LinkGuidProxy : MonoBehaviour, IConvertGameObjectToEntity {
    public string guidSource;
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
        dstManager.RemoveComponent<LocalToWorld>(entity);
        dstManager.RemoveComponent<Rotation>(entity);
        dstManager.RemoveComponent<Translation>(entity);

        dstManager.AddComponentData(entity, new GuidComponent() {
            value = Utility.GetHashCode(guidSource)
        });

        var spritePreset = GetComponent<SpritePreset>();
        if (false == ReferenceEquals(null, spritePreset)) {
            dstManager.AddSharedComponentData(entity, new SpritePresetComponent() {
                value = spritePreset
            });
        }
    }
}
