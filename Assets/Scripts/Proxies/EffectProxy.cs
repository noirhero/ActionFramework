// Copyright 2018-2020 TAP, Inc. All Rights Reserved/

using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class EffectProxy : MonoBehaviour, IConvertGameObjectToEntity {
    public string asePath;
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
        dstManager.AddComponentData(entity, new FindSpritePresetComponent() {
            value = Utility.GetHashCode(asePath)
        });

        dstManager.AddComponentData(entity,  new AnimationFrameComponent {
            state = AnimUtility.run,
        });
    }
}
