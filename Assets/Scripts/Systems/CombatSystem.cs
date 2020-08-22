// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using Unity.Entities;

public class CombatSystem : ComponentSystem {
    private bool _hasBeenInitialized = false;

    protected override void OnStartRunning() {
        if (_hasBeenInitialized) {
            return;
        }

        _hasBeenInitialized = true;
    }

    private bool IsLocked() {
        if (Entity.Null == Utility.SystemEntity) {
            return true;
        }

        if (false == EntityManager.HasComponent<InputDataComponent>(Utility.SystemEntity)) {
            return true;
        }

        // TODO : other condition
        return false;
    }

    protected override void OnUpdate() {
        if (IsLocked()) {
            return;
        }

        Entities.ForEach((Entity entity, ref ControlComponent controlComp, ref AnimationFrameComponent animComp) => {
            if (EntityManager.HasComponent<AttackComponent>(entity)) {
                if (1.0f <= animComp.frameRate) {
                    EntityManager.RemoveComponent<AttackComponent>(entity);
                }

                if (false == AnimUtility.HasState(animComp, AnimUtility.attack)) {
                    animComp.state |= AnimUtility.attack;
                }
            }
            else {
                if (AnimUtility.HasState(animComp, AnimUtility.attack)) {
                    animComp.state ^= AnimUtility.attack;
                }
            }
        });
    }
}
