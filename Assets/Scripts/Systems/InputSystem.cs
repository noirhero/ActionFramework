// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.InputSystem;

public class InputSystem : ComponentSystem, InputActions.ICharacterControlActions {
    private bool IsLocked() {
        if (false == EntityManager.HasComponent<InputDataComponent>(_inputEntity)) {
            return true;
        }

        if (false == EntityManager.HasComponent<AnimationFrameComponent>(_controlEntity)) {
            return true;
        }

        // TODO : other condition
        return false;
    }

    public void OnLeft(InputAction.CallbackContext context) {
        if (IsLocked()) {
            return;
        }

        var dataComp = EntityManager.GetComponentData<InputDataComponent>(_inputEntity);

        if (context.started) {
            dataComp.state |= InputUtility.left;
            dataComp.dir += -1.0f;
        }

        if (InputUtility.HasState(dataComp, InputUtility.left) &&
            context.canceled) {
            dataComp.state ^= InputUtility.left;
            dataComp.dir += 1.0f;
        }

        EntityManager.SetComponentData(_inputEntity, dataComp);
    }

    public void OnRight(InputAction.CallbackContext context) {
        if (IsLocked()) {
            return;
        }

        var dataComp = EntityManager.GetComponentData<InputDataComponent>(_inputEntity);

        if (context.started) {
            dataComp.state |= InputUtility.right;
            dataComp.dir += 1.0f;
        }

        if (InputUtility.HasState(dataComp, InputUtility.right) &&
            context.canceled) {
            dataComp.state ^= InputUtility.right;
            dataComp.dir += -1.0f;
        }

        EntityManager.SetComponentData(_inputEntity, dataComp);
    }

    public void OnJump(InputAction.CallbackContext context) {
        if (IsLocked()) {
            return;
        }

        var dataComp = EntityManager.GetComponentData<InputDataComponent>(_inputEntity);

        if (context.started) {
            dataComp.state |= InputUtility.jump;
        }

        if (InputUtility.HasState(dataComp, InputUtility.jump) &&
            context.canceled) {
            dataComp.state ^= InputUtility.jump;
        }

        EntityManager.SetComponentData(_inputEntity, dataComp);
    }

    public void OnAttack(InputAction.CallbackContext context) {
        if (IsLocked()) {
            return;
        }

        var dataComp = EntityManager.GetComponentData<InputDataComponent>(_inputEntity);

        if (context.started) {
            dataComp.state |= InputUtility.attack;
        }

        if (InputUtility.HasState(dataComp, InputUtility.attack) &&
            context.canceled) {
            dataComp.state ^= InputUtility.attack;
        }

        EntityManager.SetComponentData(_inputEntity, dataComp);
    }

    public void OnLeftRightAxis(InputAction.CallbackContext context) {
        if (IsLocked()) {
            return;
        }

        var dataComp = EntityManager.GetComponentData<InputDataComponent>(_inputEntity);
        dataComp.dir = context.ReadValue<float>();

        if (0.0f != dataComp.dir) {
            dataComp.state |= InputUtility.axis;
        }
        else {
            dataComp.state ^= InputUtility.axis;
        }

        EntityManager.SetComponentData(_inputEntity, dataComp);
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

    protected override void OnUpdate() {
        if (IsLocked()) {
            return;
        }

        var moveComp = EntityManager.GetComponentData<MoveComponent>(_controlEntity);
        var inputDataComp = EntityManager.GetComponentData<InputDataComponent>(_inputEntity);
        var animComp = EntityManager.GetComponentData<AnimationFrameComponent>(_controlEntity);

        // run
        moveComp.value.x = AnimUtility.IsChangeAnim(animComp, AnimUtility.run) ? inputDataComp.dir : 0.0f;

        // jump
        if (AnimUtility.IsChangeAnim(animComp, AnimUtility.jump) &&
            InputUtility.HasState(inputDataComp, InputUtility.jump)) {
            moveComp.value.y = Utility.force;

            // should be once play
            inputDataComp.state ^= InputUtility.jump;

            EntityManager.AddComponentData(_controlEntity, new JumpComponent());
        }

        // attack
        if (AnimUtility.IsChangeAnim(animComp, AnimUtility.attack) &&
            InputUtility.HasState(inputDataComp, InputUtility.attack)) {
            if (false == EntityManager.HasComponent<AttackComponent>(_controlEntity)) {
                EntityManager.AddComponentData(_controlEntity, new AttackComponent());
            }

            // should be once play
            inputDataComp.state ^= InputUtility.attack;
        }

        // gravity
        moveComp.value.y = math.max(moveComp.value.y - Utility.gravity, Utility.terminalVelocity);
        EntityManager.SetComponentData(_controlEntity, moveComp);

        EntityManager.SetComponentData(_inputEntity, inputDataComp);
    }
}
