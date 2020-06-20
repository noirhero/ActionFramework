// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using Unity.Entities;
using UnityEngine;

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

        if (EntityManager.HasComponent<JumpComponent>(_inputEntity)) {
            return true;
        }

        if (EntityManager.HasComponent<FallComponent>(_inputEntity)) {
            return true;
        }

        // TODO : other condition
        return false;
    }

    protected override void OnUpdate() {
        if (IsLocked()) {
            return;
        }

        bool bIsStop = true;
        if (TryAttack()) {
            var animComp = EntityManager.GetComponentData<AnimationFrameComponent>(_controlEntity);
            animComp.setId = Utility.AnimState.Attack;
            animComp.bLooping = true;
            EntityManager.SetComponentData(_controlEntity, animComp);

            bIsStop = false;
        }

        if (bIsStop) {
            var animComp = EntityManager.GetComponentData<AnimationFrameComponent>(_controlEntity);
            animComp.setId = Utility.AnimState.Idle;
            animComp.bLooping = true;
            EntityManager.SetComponentData(_controlEntity, animComp);
        }
    }

    private bool TryAttack() {
        if (EntityManager.HasComponent<AttackComponent>(_inputEntity)) {
            var attackComp = EntityManager.GetComponentData<AttackComponent>(_inputEntity);

            if (Utility.bShowInputLog) {
                Debug.Log("Attack : " + attackComp.accumTime);
            }

            return true;
        }

        return false;
    }
}
