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

    protected override void OnUpdate() {
        if (GameOver.bIsOvered == hasBeenOvered) {
            return;
        }

        hasBeenOvered = GameOver.bIsOvered;

        if (GameOver.bIsOvered) {
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

                    if (false == EntityManager.HasComponent<GUIComponent>(Utility.SystemEntity)) {
                        EntityManager.AddComponentData(Utility.SystemEntity, new GUIComponent() {
                            id = IdUtility.GUIId.Result
                        });
                    }
                })
                .Run();
        }
    }
}
