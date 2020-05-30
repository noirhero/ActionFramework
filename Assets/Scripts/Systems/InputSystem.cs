// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputSystem : ComponentSystem, InputActions.ICharacterControlActions {
    private bool IsLockedInput() {
        if (false == EntityManager.HasComponent<InputDataComponent>(_inputEntity)) {
            return true;
        }
        
        //TODO Condition
        return false;
    }
    
    public void OnLeft(InputAction.CallbackContext context) {
        if (IsLockedInput()) {
            return;
        }

        var cachedComp = EntityManager.GetComponentData<InputDataComponent>(_inputEntity);
        if (context.started) {
            cachedComp.state |= InputState.left;
            cachedComp.dir += -1.0f;
        }
        if (context.canceled) {
            cachedComp.state ^= InputState.left;
            cachedComp.dir += 1.0f;
        }
        EntityManager.SetComponentData(_inputEntity, cachedComp);
    }

    public void OnRight(InputAction.CallbackContext context) {
        if (IsLockedInput()) {
            return;
        }
        
        var cachedComp = EntityManager.GetComponentData<InputDataComponent>(_inputEntity);
        if (context.started) {
            cachedComp.state |= InputState.right;
            cachedComp.dir += 1.0f;
        }
        if (context.canceled) {
            cachedComp.state ^= InputState.right;
            cachedComp.dir += -1.0f;
        }
        EntityManager.SetComponentData(_inputEntity, cachedComp);
    }

    public void OnJump(InputAction.CallbackContext context) {
        if (IsLockedInput()) {
            return;
        }
        
        var cachedComp = EntityManager.GetComponentData<InputDataComponent>(_inputEntity);
        if (context.started) {
            cachedComp.state |= InputState.jump;
        }
        if (context.canceled) {
            cachedComp.state ^= InputState.jump;
        }
        EntityManager.SetComponentData(_inputEntity, cachedComp);
    }

    public void OnAttack(InputAction.CallbackContext context) {
        if (IsLockedInput()) {
            return;
        }
        
        var cachedComp = EntityManager.GetComponentData<InputDataComponent>(_inputEntity);
        if (context.started) {
            cachedComp.state |= InputState.attack;
        }
        if (context.canceled) {
            cachedComp.state ^= InputState.attack;
        }
        EntityManager.SetComponentData(_inputEntity, cachedComp);
    }

    public void OnLeftRightAxis(InputAction.CallbackContext context) {
        if (IsLockedInput()) {
            return;
        }
        
        var cachedComp = EntityManager.GetComponentData<InputDataComponent>(_inputEntity);
        cachedComp.dir = context.ReadValue<float>();
        if (0.0f != cachedComp.dir) {
            cachedComp.state |= InputState.axis;
        }
        else {
            cachedComp.state ^= InputState.axis;
        }

        EntityManager.SetComponentData(_inputEntity, cachedComp);
    }

    private InputActions _input;
    private Entity _inputEntity;
    protected override void OnCreate() {
        _input = new InputActions();
        _input.CharacterControl.SetCallbacks(this);

        _inputEntity = EntityManager.CreateEntity();
        EntityManager.AddComponentData(_inputEntity, new InputDataComponent());
    }

    protected override void OnStartRunning() {
        _input.Enable();
    }

    protected override void OnStopRunning() {
        _input.Disable();
    }

    protected override void OnUpdate() {
        if (IsLockedInput()) {
            return;
        }
        
        UpdateMove();
        UpdateJump();
        UpdateAttack();

        if (Utility.bShowInputLog) {
            var inputDataComp = EntityManager.GetComponentData<InputDataComponent>(_inputEntity);
            var cachedLog = InputState.ShowLog(inputDataComp);
            if (false == string.IsNullOrEmpty(cachedLog)) {
                Debug.Log("Current Input State :" + cachedLog);
            }
        }
    }

    private void UpdateMove() {
        var inputDataComp = EntityManager.GetComponentData<InputDataComponent>(_inputEntity);
        
        if (0.0f != inputDataComp.dir) {
            if (false == EntityManager.HasComponent<InputMoveComponent>(_inputEntity)) {
                EntityManager.AddComponentData(_inputEntity, new InputMoveComponent());
            }

            var cachedComp = EntityManager.GetComponentData<InputMoveComponent>(_inputEntity);
            if ((0.0f > cachedComp.value.x && 0.0f > inputDataComp.dir) ||
                (0.0f < cachedComp.value.x && 0.0f < inputDataComp.dir)) {
                cachedComp.accumTime += Time.DeltaTime;
            }
            else {
                cachedComp.accumTime = 0.0f;
            }
            cachedComp.value.x = inputDataComp.dir;
            EntityManager.SetComponentData(_inputEntity, cachedComp);
        }
        else {
            if (EntityManager.HasComponent<InputMoveComponent>(_inputEntity)) {
                EntityManager.RemoveComponent<InputMoveComponent>(_inputEntity);
            }
        }
    }

    private void UpdateJump() {
        var inputDataComp = EntityManager.GetComponentData<InputDataComponent>(_inputEntity);
        
        if (InputState.HasState(inputDataComp, InputState.jump)) {
            if (false == EntityManager.HasComponent<InputJumpComponent>(_inputEntity)) {
                EntityManager.AddComponentData(_inputEntity, new InputJumpComponent());
            }

            var cachedComp = EntityManager.GetComponentData<InputJumpComponent>(_inputEntity);
            cachedComp.accumTime = Time.DeltaTime;
            EntityManager.SetComponentData(_inputEntity, cachedComp);
        }
        else {
            if (EntityManager.HasComponent<InputJumpComponent>(_inputEntity)) {
                EntityManager.RemoveComponent<InputJumpComponent>(_inputEntity);
            }
        }
    }

    private void UpdateAttack() {
        var inputDataComp = EntityManager.GetComponentData<InputDataComponent>(_inputEntity);
        
        if (InputState.HasState(inputDataComp, InputState.attack)) {
            if (false == EntityManager.HasComponent<InputAttackComponent>(_inputEntity)) {
                EntityManager.AddComponentData(_inputEntity, new InputAttackComponent());
            }

            var cachedComp = EntityManager.GetComponentData<InputAttackComponent>(_inputEntity);
            cachedComp.accumTime = Time.DeltaTime;
            EntityManager.SetComponentData(_inputEntity, cachedComp);
        }
        else {
            if (EntityManager.HasComponent<InputAttackComponent>(_inputEntity)) {
                EntityManager.RemoveComponent<InputAttackComponent>(_inputEntity);
            }
        }
    }
}
