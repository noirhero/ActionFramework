// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using Unity.Entities;

public class GUISystem : ComponentSystem {
    private IdUtility.GUIId _uiState = IdUtility.GUIId.None;
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

                    EntityManager.AddComponentData(EntityManager.CreateEntity(), new SubSceneLoadComponent {
                        id = IdUtility.GUIId.Title,
                        delay = 0.0f
                    });
                    break;
                case IdUtility.GUIId.InGame:
                    GameStart.Play();
                    GameOver.Play();

                    if (false == EntityManager.HasComponent<FadeOutComponent>(Utility.SystemEntity)) {
                        EntityManager.AddComponentData(Utility.SystemEntity, new FadeOutComponent() {
                            time = 0.5f
                        });
                    }

                    EntityManager.AddComponentData(EntityManager.CreateEntity(), new SubSceneUnLoadComponent {
                        id = IdUtility.GUIId.Title,
                        delay = 0.0f
                    });
                    EntityManager.AddComponentData(EntityManager.CreateEntity(), new SubSceneUnLoadComponent {
                        id = IdUtility.GUIId.Result,
                        delay = 0.0f
                    });
                    EntityManager.AddComponentData(EntityManager.CreateEntity(), new SubSceneLoadComponent {
                        id = IdUtility.GUIId.InGame,
                        delay = 0.0f
                    });
                    break;
                case IdUtility.GUIId.Result:
                    GameStart.Stop();

                    if (false == EntityManager.HasComponent<FadeInComponent>(Utility.SystemEntity)) {
                        EntityManager.AddComponentData(Utility.SystemEntity, new FadeInComponent() {
                            time = 2.0f
                        });
                    }

                    EntityManager.AddComponentData(EntityManager.CreateEntity(), new SubSceneUnLoadComponent {
                        id = IdUtility.GUIId.InGame,
                        delay = 2.0f
                    });
                    EntityManager.AddComponentData(EntityManager.CreateEntity(), new SubSceneLoadComponent {
                        id = IdUtility.GUIId.Result,
                        delay = 1.0f
                    });
                    break;
            }

            _uiState = guiComp.id;

            EntityManager.RemoveComponent<GUIComponent>(entity);
        });
    }
}
