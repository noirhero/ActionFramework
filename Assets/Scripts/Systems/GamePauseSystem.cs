// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using UnityEngine;
using Unity.Entities;

public class GamePauseSystem : SystemBase {
    public bool hasBeenPaused = false;
    public System.Type[] systemTypes = null;
    public int systemsMaxNum = 15;

    protected override void OnCreate() {
        var idx = 0;
        systemTypes = new System.Type[systemsMaxNum];
        systemTypes[idx++] = typeof(ControllerSystem);
        systemTypes[idx++] = typeof(AnimationFrameSystem);
        systemTypes[idx++] = typeof(AutoDestroySystem);
        systemTypes[idx++] = typeof(CameraFollowSystem);
        systemTypes[idx++] = typeof(CollisionSystem);
        systemTypes[idx++] = typeof(CombatSystem);
        systemTypes[idx++] = typeof(FindSpritePresetSystem);
        systemTypes[idx++] = typeof(HitSystem);
        systemTypes[idx++] = typeof(InputSystem);
        systemTypes[idx++] = typeof(MoveSystem);
        systemTypes[idx++] = typeof(SpriteChangeSystem);
        systemTypes[idx++] = typeof(TargetFollowSystem);
    }

    protected override void OnUpdate() {
        if (0.0f < Utility.gamePauseTime) {
            SetEnableSystems(false);

            Utility.gamePauseTime -= Time.DeltaTime;

            if (0.0f >= Utility.gamePauseTime) {
                Utility.gamePauseTime = 0.0f;
                SetEnableSystems(true);
            }
        }
    }

    private void SetEnableSystems(bool enabled) {
        if (hasBeenPaused != enabled) {
            return;
        }
        hasBeenPaused = !enabled;   // (enabled == false) -> pause

        foreach (var system in World.DefaultGameObjectInjectionWorld.Systems) {
            foreach (var type in systemTypes) {
                if (system.GetType() == type) {
                    system.Enabled = enabled;
                }
            }
        }
    }
}
