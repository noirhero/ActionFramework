// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using Unity.Entities;

public class CombatSystem : ComponentSystem {
    private Entity _inputEntity;
    private Entity _controlEntity;

    protected override void OnStartRunning() {
        Entities.ForEach((Entity inputEntity, ref InputDataComponent dataComp) => { _inputEntity = inputEntity; });
        Entities.ForEach((Entity controlEntity, ref InputComponent inputComp) => { _controlEntity = controlEntity; });
    }

    private bool IsLocked() {
        if (Entity.Null == _inputEntity) {
            return true;
        }

        if (false == EntityManager.HasComponent<InputDataComponent>(_inputEntity)) {
            return true;
        }

        if (Entity.Null == _controlEntity) {
            return true;
        }

        if (false == EntityManager.HasComponent<AnimationFrameComponent>(_controlEntity)) {
            return true;
        }

        if (EntityManager.HasComponent<MoveComponent>(_inputEntity)) {
            return true;
        }
        // TODO : other condition
        return false;
    }

    protected override void OnUpdate() {
        if (IsLocked()) {
            return;
        }

        var animComp = EntityManager.GetComponentData<AnimationFrameComponent>(_controlEntity);
        if (EntityManager.HasComponent<AttackComponent>(_controlEntity)) {
            EntityManager.RemoveComponent<AttackComponent>(_controlEntity);
            
            if (false == AnimState.HasState(animComp, AnimState.attack)) {
                animComp.state |= AnimState.attack;   
            }
        }
        else {
            if (AnimState.HasState(animComp, AnimState.attack)) {
                animComp.state ^= AnimState.attack;   
            }
        }
        EntityManager.SetComponentData(_controlEntity, animComp);
    }
}
