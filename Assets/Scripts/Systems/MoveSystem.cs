// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class MoveSystem : ComponentSystem {
    private Entity _inputEntity;
    private Entity _controlEntity; // TODO : choice move control entity 
    
    protected override void OnStartRunning() {
        Entities.ForEach((Entity inputEntity, ref InputDataComponent dataComp) => {
            _inputEntity = inputEntity;
        });
        Entities.ForEach((Entity controlEntity, ref InputComponent inputComp) => {
            _controlEntity = controlEntity;
        });
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

    protected override void OnUpdate() {
        if (IsLocked()) {
            return;
        }

        bool bIsStop = true;
        if (TryMove()) {
            var animComp = EntityManager.GetComponentData<AnimationFrameComponent>(_controlEntity);
            animComp.setId = Utility.AnimState.Run;
            animComp.bLooping = true;
            EntityManager.SetComponentData(_controlEntity, animComp);

            bIsStop = false;
        }
        if (TryJump()) {
            var animComp = EntityManager.GetComponentData<AnimationFrameComponent>(_controlEntity);
            animComp.setId = Utility.AnimState.Jump;
            animComp.bLooping = false;
            EntityManager.SetComponentData(_controlEntity, animComp);
            
            bIsStop = false;
        }
        if (TryFall()) {
            var animComp = EntityManager.GetComponentData<AnimationFrameComponent>(_controlEntity);
            animComp.setId = Utility.AnimState.Jump;
            animComp.bLooping = false;
            EntityManager.SetComponentData(_controlEntity, animComp);
            
            bIsStop = false;
        }
        
        if (bIsStop) {
            var animComp = EntityManager.GetComponentData<AnimationFrameComponent>(_controlEntity);
            animComp.setId = Utility.AnimState.Idle;
            animComp.bLooping = true;
            animComp.lockFrameIndex = Utility.INDEX_NONE;
            EntityManager.SetComponentData(_controlEntity, animComp);
        }
    }
    
    // TODO : temporary constant -> status 
    private const float _speedX = 0.01f;
    private const float _speedY = 0.1f;
    private const float gravity = 0.3f;
    
    private bool TryMove() {
        if (EntityManager.HasComponent<MoveComponent>(_inputEntity)) {
            var moveComp = EntityManager.GetComponentData<MoveComponent>(_inputEntity);
            
            var transComp = EntityManager.GetComponentData<Translation>(_controlEntity);
            transComp.Value.x += moveComp.value.x * _speedX;
            EntityManager.SetComponentData(_controlEntity, transComp);
            
            var animComp = EntityManager.GetComponentData<AnimationFrameComponent>(_controlEntity);
            animComp.bFlipX = moveComp.value.x < 0.0f;
            animComp.lockFrameIndex = Utility.INDEX_NONE;
            EntityManager.SetComponentData(_controlEntity, animComp);
            
            if (Utility.bShowInputLog) {
                Debug.Log("Move : " + moveComp.value.x + " ("+moveComp.accumTime+")");
            }
            return true;
        }
        return false;
    }
    
    private bool TryJump() {
        if (EntityManager.HasComponent<JumpComponent>(_inputEntity)) {
            var jumpComp = EntityManager.GetComponentData<JumpComponent>(_inputEntity);
            jumpComp.force -= gravity;
            jumpComp.lastDeltaY = jumpComp.force * Time.DeltaTime * _speedY;
            jumpComp.accumY += jumpComp.lastDeltaY;
            EntityManager.SetComponentData(_inputEntity, jumpComp);
            
            var transComp = EntityManager.GetComponentData<Translation>(_controlEntity);
            transComp.Value.y += jumpComp.lastDeltaY;
            EntityManager.SetComponentData(_controlEntity, transComp);
            
            var animComp = EntityManager.GetComponentData<AnimationFrameComponent>(_controlEntity);
            animComp.lockFrameIndex = 0.0f < jumpComp.force ? 2 : Utility.INDEX_NONE; // TODO : temporary setting
            EntityManager.SetComponentData(_controlEntity, animComp);
            
            if (Utility.bShowInputLog) {
                Debug.Log("Jump force ("+jumpComp.force+"/" +jumpComp.accumY+"/" +transComp.Value.y+")");
            }
            return true;
        }
        return false;
    }
    
    private bool TryFall() {
        if (EntityManager.HasComponent<FallComponent>(_inputEntity)) {
            var fallComp = EntityManager.GetComponentData<FallComponent>(_inputEntity);
            fallComp.force -= gravity;
            var deltaY = fallComp.force * Time.DeltaTime * _speedY;
            fallComp.accumY += deltaY;
            EntityManager.SetComponentData(_inputEntity, fallComp);
            
            var transComp = EntityManager.GetComponentData<Translation>(_controlEntity);
            transComp.Value.y -= deltaY;
            EntityManager.SetComponentData(_controlEntity, transComp);
            
            if (Utility.bShowInputLog) {
                Debug.Log("Fall force ("+fallComp.force+"/" +fallComp.accumY+"/" +transComp.Value.y+")");
            }
            return true;
        }
        return false;
    }
}
