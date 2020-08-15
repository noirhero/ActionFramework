// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using Unity.Entities;

public static class GameOver {
    public static bool bIsOvered { get; private set; }

    public static void Over() {
        bIsOvered = true;
    }
    public static void Play() {
        bIsOvered = false;
    }
}

public class GameOverSystem : SystemBase {
    public bool hasBeenOvered = false;
    public System.Type[] systemTypes = null;
    public int systemsMaxNum = 15;
    
    protected override void OnCreate() {
        GameOver.Play();
        
        var idx = 0;
        systemTypes = new System.Type[systemsMaxNum];
        systemTypes[idx++] = typeof(CollisionSystem);
        systemTypes[idx++] = typeof(CombatSystem);
        systemTypes[idx++] = typeof(HitSystem);
        systemTypes[idx++] = typeof(MoveSystem);
        systemTypes[idx++] = typeof(TargetFollowSystem);
    }
    
    protected override void OnUpdate() {
        if (GameOver.bIsOvered == hasBeenOvered) {
            return;
        }
            
        if (GameOver.bIsOvered) {
            SetEnableSystems(false);
            Entities.WithName("GameOverSystem")
                .WithoutBurst()
                .WithStructuralChanges()
                .ForEach((Entity entity, ref TargetIdComponent idComp, ref AnimationFrameComponent animComp) => {
                    if (IdUtility.Id.Player != idComp.value) {
                        return;
                    }
                
                    if (false == AnimUtility.HasState(animComp, AnimUtility.hit)) {
                        animComp.state |= AnimUtility.hit;
                    }
                })
                .Run();
        }
        else {
            SetEnableSystems(true);
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
