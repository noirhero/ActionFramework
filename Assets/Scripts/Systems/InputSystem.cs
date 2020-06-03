// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputSystem : ComponentSystem, InputActions.ICharacterControlActions {
    private bool IsLocked() {
        if (false == EntityManager.HasComponent<InputDataComponent>(_inputEntity)) {
            return true;
        }
        
        // TODO : other condition
        return false;
    }
    
    public void OnLeft(InputAction.CallbackContext context) {
        if (IsLocked()) {
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
        if (IsLocked()) {
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
        if (IsLocked()) {
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
        if (IsLocked()) {
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
        if (IsLocked()) {
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
        if (IsLocked()) {
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
            if (false == EntityManager.HasComponent<MoveComponent>(_inputEntity)) {
                EntityManager.AddComponentData(_inputEntity, new MoveComponent());
            }

            var cachedComp = EntityManager.GetComponentData<MoveComponent>(_inputEntity);
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
            if (EntityManager.HasComponent<MoveComponent>(_inputEntity)) {
                EntityManager.RemoveComponent<MoveComponent>(_inputEntity);
            }
        }
    }

    private void UpdateJump() {
        var inputDataComp = EntityManager.GetComponentData<InputDataComponent>(_inputEntity);
        if (EntityManager.HasComponent<JumpComponent>(_inputEntity)) {
            var jumpComp = EntityManager.GetComponentData<JumpComponent>(_inputEntity);
            // TODO : temporary limit check -> check collision
            if (1.0f < jumpComp.accumY) {
                EntityManager.RemoveComponent<JumpComponent>(_inputEntity);
                EntityManager.AddComponentData(_inputEntity, new FallComponent(10.0f));
            }
        }
        else if (EntityManager.HasComponent<FallComponent>(_inputEntity)) {
            var fallComp = EntityManager.GetComponentData<FallComponent>(_inputEntity);
            // TODO : temporary limit check -> check collision
            if (1.0f < fallComp.accumY) {
                EntityManager.RemoveComponent<FallComponent>(_inputEntity);
            }
        }
        else if (InputState.HasState(inputDataComp, InputState.jump)) {
            EntityManager.AddComponentData(_inputEntity, new JumpComponent(10.0f));
        }
    }

    private void UpdateAttack() {
        var inputDataComp = EntityManager.GetComponentData<InputDataComponent>(_inputEntity);
        if (InputState.HasState(inputDataComp, InputState.attack)) {
            if (false == EntityManager.HasComponent<AttackComponent>(_inputEntity)) {
                EntityManager.AddComponentData(_inputEntity, new AttackComponent());
            }

            var cachedComp = EntityManager.GetComponentData<AttackComponent>(_inputEntity);
            cachedComp.accumTime = Time.DeltaTime;
            EntityManager.SetComponentData(_inputEntity, cachedComp);
        }
        else {
            if (EntityManager.HasComponent<AttackComponent>(_inputEntity)) {
                EntityManager.RemoveComponent<AttackComponent>(_inputEntity);
            }
        }
    }
}
