// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class MoveSystem : ComponentSystem {
    private bool IsLockedMove() {
        if (Entity.Null == _inputEntity) {
            return true;
        }
        if (false == EntityManager.HasComponent<InputDataComponent>(_inputEntity)) {
            return true;
        }
        
        if (Entity.Null == _controlEntity) {
            return true;
        }
        if (false == EntityManager.HasComponent<Translation>(_controlEntity)) {
            return true;
        }
        if (false == EntityManager.HasComponent<AnimationFrameComponent>(_controlEntity)) {
            return true;
        }
        
        //TODO Condition
        return false;
    }
    
    private Entity _inputEntity;
    private Entity _controlEntity;
    protected override void OnStartRunning() {
        Entities.ForEach((Entity inputEntity, ref InputDataComponent dataComp) => {
            _inputEntity = inputEntity;
        });
        Entities.ForEach((Entity controlEntity, ref InputComponent inputComp) => {
            _controlEntity = controlEntity;
        });
    }

    private const float _speedX = 0.01f;
    protected override void OnUpdate() {
        if (IsLockedMove()) {
            return;
        }
        
        if (EntityManager.HasComponent<InputMoveComponent>(_inputEntity)) {
            var inputComp = EntityManager.GetComponentData<InputMoveComponent>(_inputEntity);
            if (Utility.bShowInputLog) {
                Debug.Log("Move : " + inputComp.value.x + " (+"+inputComp.accumTime+")");
            }

            var moveComp = EntityManager.GetComponentData<Translation>(_controlEntity);
            moveComp.Value.x += inputComp.value.x * _speedX;
            EntityManager.SetComponentData(_controlEntity, moveComp);

            var animComp = EntityManager.GetComponentData<AnimationFrameComponent>(_controlEntity);
            animComp.setId = Utility.AnimState.Run;
            animComp.flipX = inputComp.value.x < 0.0f;
            EntityManager.SetComponentData(_controlEntity, animComp);
        }
        else {

            var animComp = EntityManager.GetComponentData<AnimationFrameComponent>(_controlEntity);
            animComp.setId = Utility.AnimState.Idle;
            EntityManager.SetComponentData(_controlEntity, animComp);
        }
    }
}
