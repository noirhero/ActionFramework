// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using Unity.Entities;

public static class GameStart {
    public static bool bIsPlayed { get; private set; }

    public static void Stop() {
        bIsPlayed = false;
    }

    public static void Play() {
        bIsPlayed = true;
    }
}

public class GameStartSystem : SystemBase {
    public bool hasBeenPlayed = true;
    public System.Type[] systemTypes = null;
    public int systemsMaxNum = 15;

    protected override void OnCreate() {
        GameStart.Play();

        var idx = 0;
        systemTypes = new System.Type[systemsMaxNum];
        systemTypes[idx++] = typeof(CollisionSystem);
        systemTypes[idx++] = typeof(CombatSystem);
        systemTypes[idx++] = typeof(HitSystem);
        systemTypes[idx++] = typeof(MoveSystem);
        systemTypes[idx++] = typeof(TargetFollowSystem);
    }

    protected override void OnUpdate() {
        if (GameStart.bIsPlayed == hasBeenPlayed) {
            return;
        }

        hasBeenPlayed = GameStart.bIsPlayed;

        if (GameStart.bIsPlayed) {
            SetEnableSystems(true);
        }
        else {
            SetEnableSystems(false);
        }
    }

    private void SetEnableSystems(bool enabled) {
        foreach (var system in World.DefaultGameObjectInjectionWorld.Systems) {
            foreach (var type in systemTypes) {
                if (system.GetType() == type) {
                    system.Enabled = enabled;
                }
            }
        }
    }
}
