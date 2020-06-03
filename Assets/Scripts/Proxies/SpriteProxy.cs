// Copyright 2018-2020 TAP, Inc. All Rights Reserved/

using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class SpriteProxy : MonoBehaviour, IConvertGameObjectToEntity {
    public string asePath;
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
        dstManager.AddComponentData(entity, new FindSpritePresetComponent() {
            value = Utility.GetHashCode(asePath)
        });

        // TODO : temporary added
        dstManager.AddComponentData(entity, new InputComponent());
    }
}
