// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using Unity.Entities;

public class ConfirmSystem : ComponentSystem {
    protected override void OnUpdate() {
        Entities.ForEach((Entity entity, ref ConfirmComponent guiComp) => {

            switch (Utility.GetGameState()) {
                case IdUtility.GameStateId.Title:
                    if (false == EntityManager.HasComponent<InstantAudioComponent>(Utility.SystemEntity)) {
                        EntityManager.AddComponentData(Utility.SystemEntity, new InstantAudioComponent() {
                            id = SoundUtility.ClipKey.Button,
                        });
                    }
                    if (false == EntityManager.HasComponent<GUIComponent>(Utility.SystemEntity)) {
                        EntityManager.AddComponentData(Utility.SystemEntity, new GUIComponent() {
                            id = IdUtility.GUIId.InGame
                        });
                    }
                    break;
                case IdUtility.GameStateId.Play:
                    GameStart.Stop();
                    break;
                case IdUtility.GameStateId.Pause:
                    GameStart.Play();
                    break;
                case IdUtility.GameStateId.Result:
                    if (false == EntityManager.HasComponent<InstantAudioComponent>(Utility.SystemEntity)) {
                        EntityManager.AddComponentData(Utility.SystemEntity, new InstantAudioComponent() {
                            id = SoundUtility.ClipKey.Button,
                        });
                    }
                    if (false == EntityManager.HasComponent<GUIComponent>(Utility.SystemEntity)) {
                        EntityManager.AddComponentData(Utility.SystemEntity, new GUIComponent() {
                            id = IdUtility.GUIId.InGame
                        });
                    }
                    break;
            }

            EntityManager.RemoveComponent<ConfirmComponent>(entity);
        });
    }
}
