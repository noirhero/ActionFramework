// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using Unity.Entities;
using Unity.Mathematics;
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

        if (InputState.HasState(cachedComp, InputState.left) && context.canceled) {
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

        if (InputState.HasState(cachedComp, InputState.right) && context.canceled) {
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

        if (InputState.HasState(cachedComp, InputState.jump) && context.canceled) {
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

        if (InputState.HasState(cachedComp, InputState.attack) && context.canceled) {
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
    private Entity _controlEntity; // TODO : choice move control entity 

    protected override void OnCreate() {
        _input = new InputActions();
        _input.CharacterControl.SetCallbacks(this);

        _inputEntity = EntityManager.CreateEntity();
        EntityManager.AddComponentData(_inputEntity, new InputDataComponent());
    }

    protected override void OnStartRunning() {
        _input.Enable();
        Entities.ForEach((Entity controlEntity, ref MoveComponent moveComp) => { _controlEntity = controlEntity; });
    }

    protected override void OnStopRunning() {
        _input.Disable();
    }

    private const float _force = 50.0f;
    private const float _gravity = 2.0f;

    protected override void OnUpdate() {
        if (IsLocked()) {
            return;
        }

        var moveComp = EntityManager.GetComponentData<MoveComponent>(_controlEntity);
        var inputDataComp = EntityManager.GetComponentData<InputDataComponent>(_inputEntity);

        //jump
        if (EntityManager.HasComponent<JumpComponent>(_controlEntity)) {
            if (0.0f > moveComp.value.y) {
                EntityManager.RemoveComponent<JumpComponent>(_controlEntity);
            }
        }
        else {
            if (InputState.HasState(inputDataComp, InputState.jump)) {
                moveComp.value.y = _force;

                // should be once play
                inputDataComp.state ^= InputState.jump;
                EntityManager.SetComponentData(_inputEntity, inputDataComp);

                EntityManager.AddComponentData(_controlEntity, new JumpComponent());
            }
        }

        moveComp.value.x = inputDataComp.dir;
        moveComp.value.y = math.max(moveComp.value.y - _gravity, -20.0f);
        EntityManager.SetComponentData(_controlEntity, moveComp);

        // attack
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

        // log
        if (Utility.bShowInputLog) {
            var cachedLog = InputState.ShowLog(inputDataComp);
            if (false == string.IsNullOrEmpty(cachedLog)) {
                Debug.Log("Current Input State :" + cachedLog);
            }
        }
    }
}