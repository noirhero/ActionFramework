// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputSystem : ComponentSystem, InputActions.ICharacterControlActions {
    private bool IsLockedInput() {
        if (false != EntityManager.HasComponent<InputDataComponent>(_inputEntity)) {
            return false;
        }
        
        //TODO Condition
        return true;
    }
    
    public void OnLeft(InputAction.CallbackContext context) {
        if (IsLockedInput()) {
            return;
        }

        var cachedComp = EntityManager.GetComponentData<InputDataComponent>(_inputEntity);
        if (context.started) {
            cachedComp.state |= InputState.left;
        }
        if (context.canceled) {
            cachedComp.state ^= InputState.left;
        }
        EntityManager.SetComponentData(_inputEntity, cachedComp);
        
        bDirty = true;
    }

    public void OnRight(InputAction.CallbackContext context) {
        if (IsLockedInput()) {
            return;
        }
        
        var cachedComp = EntityManager.GetComponentData<InputDataComponent>(_inputEntity);
        if (context.started) {
            cachedComp.state |= InputState.right;
        }
        if (context.canceled) {
            cachedComp.state ^= InputState.right;
        }
        EntityManager.SetComponentData(_inputEntity, cachedComp);
        
        bDirty = true;
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
        
        bDirty = true;
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
        
        bDirty = true;
    }

    private Vector2 _axisValue;
    public void OnLeftRightAxis(InputAction.CallbackContext context) {
        if (IsLockedInput()) {
            return;
        }

        _axisValue.x = context.ReadValue<float>();
        var cachedComp = EntityManager.GetComponentData<InputDataComponent>(_inputEntity);
        if (0.0f != _axisValue.x) {
            cachedComp.state |= InputState.axis;
        }
        else {
            cachedComp.state ^= InputState.axis;
        }
        EntityManager.SetComponentData(_inputEntity, cachedComp);
        
        bDirty = true;
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

    private bool bDirty;
    protected override void OnUpdate() {
        if (IsLockedInput()) {
            return;
        }
        
        var inputDataComp = EntityManager.GetComponentData<InputDataComponent>(_inputEntity);
        if (InputState.IsNone(inputDataComp.state)) {
            return;
        }
        Debug.Log("Current Input State :" + InputState.ShowLog(inputDataComp.state));
        
        if (bDirty) {
            bDirty = false;
        }
    }
}
