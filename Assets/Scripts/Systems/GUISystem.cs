﻿// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using Unity.Entities;

public class GUISystem : ComponentSystem {
    public IdUtility.GUIId _uiState { get; private set; }

    protected override void OnUpdate() {
        Entities.ForEach((Entity entity, ref GUIComponent guiComp) => {
            if (guiComp.id == _uiState) {
                return;
            }

            var guiPreset = EntityManager.GetSharedComponentData<GUIPresetComponent>(Utility.SystemEntity);
            guiPreset.preset.GUIUpdate(guiComp.id);
            switch (guiComp.id) {
                case IdUtility.GUIId.Title:
                    GameStart.Stop();

                    if (false == EntityManager.HasComponent<FadeInComponent>(Utility.SystemEntity)) {
                        EntityManager.AddComponentData(Utility.SystemEntity, new FadeInComponent() {
                            time = 0.0f
                        });
                    }
                    break;
                case IdUtility.GUIId.InGame:
                    GameStart.Play();

                    if (false == EntityManager.HasComponent<FadeOutComponent>(Utility.SystemEntity)) {
                        EntityManager.AddComponentData(Utility.SystemEntity, new FadeOutComponent() {
                            time = 0.5f
                        });
                    }
                    break;
                case IdUtility.GUIId.Result:
                    GameStart.Stop();

                    if (false == EntityManager.HasComponent<FadeInComponent>(Utility.SystemEntity)) {
                        EntityManager.AddComponentData(Utility.SystemEntity, new FadeInComponent() {
                            time = 1.0f
                        });
                    }
                    break;
            }

            _uiState = guiComp.id;

            EntityManager.RemoveComponent<GUIComponent>(entity);
        });
    }
}