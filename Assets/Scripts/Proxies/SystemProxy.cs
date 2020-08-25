// Copyright 2018-2020 TAP, Inc. All Rights Reserved/

using Unity.Entities;
using Unity.Scenes;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class SystemProxy : MonoBehaviour, IConvertGameObjectToEntity {
    public GUIPreset guiPreset;
    public SubScene title;
    public SubScene inGame;
    public SubScene result;
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
        Utility.SetSystemEntity(entity);

        if (null != guiPreset) {
            dstManager.AddSharedComponentData(entity, new GUIPresetComponent(guiPreset));
        }

        if (title && inGame && result) {
            dstManager.AddSharedComponentData(entity, new SubSceneComponent {
                title = title,
                inGame = inGame,
                result = result
            });
        }
    }
}
