// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class MoveSystem : ComponentSystem {
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

    protected override void OnUpdate() {
        if (IsLocked()) {
            return;
        }

        bool bIsStop = true;
        if (TryMove()) {
            var animComp = EntityManager.GetComponentData<AnimationFrameComponent>(_controlEntity);
            animComp.setId = Utility.AnimState.Run;
            EntityManager.SetComponentData(_controlEntity, animComp);

            bIsStop = false;
        }
        if (TryJump()) {
            var animComp = EntityManager.GetComponentData<AnimationFrameComponent>(_controlEntity);
            animComp.setId = Utility.AnimState.Jump;
            EntityManager.SetComponentData(_controlEntity, animComp);
            
            bIsStop = false;
        }
        
        if (bIsStop) {
            var animComp = EntityManager.GetComponentData<AnimationFrameComponent>(_controlEntity);
            animComp.setId = Utility.AnimState.Idle;
            EntityManager.SetComponentData(_controlEntity, animComp);
        }
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
        if (false == EntityManager.HasComponent<Translation>(_controlEntity)) {
            return true;
        }
        if (false == EntityManager.HasComponent<AnimationFrameComponent>(_controlEntity)) {
            return true;
        }
        
        // TODO : other condition
        return false;
    }
    
    private const float _speedX = 0.01f;
    private bool TryMove() {
        if (EntityManager.HasComponent<MoveComponent>(_inputEntity)) {
            var moveComp = EntityManager.GetComponentData<MoveComponent>(_inputEntity);
            if (Utility.bShowInputLog) {
                Debug.Log("Move : " + moveComp.value.x + " (+"+moveComp.accumTime+")");
            }

            var transComp = EntityManager.GetComponentData<Translation>(_controlEntity);
            transComp.Value.x += moveComp.value.x * _speedX;
            EntityManager.SetComponentData(_controlEntity, transComp);
            
            var animComp = EntityManager.GetComponentData<AnimationFrameComponent>(_controlEntity);
            animComp.flipX = moveComp.value.x < 0.0f;
            EntityManager.SetComponentData(_controlEntity, animComp);
            return true;
        }

        return false;
    }
    
    // TODO : temporary constant -> status 
    private const float _speedY = 0.5f;
    private const float gravity = 0.1f;
    private bool TryJump() {
        if (EntityManager.HasComponent<JumpComponent>(_inputEntity)) {
            var jumpComp = EntityManager.GetComponentData<JumpComponent>(_inputEntity);
            jumpComp.force -= gravity;
            var calTransY = jumpComp.force * Time.DeltaTime * _speedY;
            jumpComp.accumY += calTransY;
            EntityManager.SetComponentData(_inputEntity, jumpComp);
            
            var transComp = EntityManager.GetComponentData<Translation>(_controlEntity);
            transComp.Value.y += calTransY;
            EntityManager.SetComponentData(_controlEntity, transComp);
            
            if (Utility.bShowInputLog) {
                Debug.Log("Jump force ("+jumpComp.force+") / (" +calTransY+")");
            }
            return true;
        }
        
        if (EntityManager.HasComponent<FallComponent>(_inputEntity)) {
            var fallComp = EntityManager.GetComponentData<FallComponent>(_inputEntity);
            fallComp.force -= gravity;
            var calTransY = fallComp.force * Time.DeltaTime * _speedY;
            fallComp.accumY += calTransY;
            EntityManager.SetComponentData(_inputEntity, fallComp);
            
            var transComp = EntityManager.GetComponentData<Translation>(_controlEntity);
            transComp.Value.y -= calTransY;
            EntityManager.SetComponentData(_controlEntity, transComp);
            
            if (Utility.bShowInputLog) {
                Debug.Log("Fall force ("+fallComp.force+") / (" +calTransY+")");
            }
            return true;
        }
        
        return false;
    }
}
