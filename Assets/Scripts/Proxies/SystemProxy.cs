// Copyright 2018-2020 TAP, Inc. All Rights Reserved/

using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class SystemProxy : MonoBehaviour, IConvertGameObjectToEntity {
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
        Utility.SetSystemEntity(entity);
    }
}
