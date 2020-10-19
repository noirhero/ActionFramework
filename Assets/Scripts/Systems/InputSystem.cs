// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Systems;
using UnityEngine.InputSystem;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class InputSystem : ComponentSystem, InputActions.ICharacterControlActions {
    public void OnLeft(InputAction.CallbackContext context) {
        var dataComp = EntityManager.GetComponentData<InputDataComponent>(Utility.SystemEntity);

        if (context.started) {
            dataComp.state |= InputUtility.left;
            dataComp.dir += -1.0f;
        }

        if (InputUtility.HasState(dataComp, InputUtility.left) && context.canceled) {
            dataComp.state ^= InputUtility.left;
            dataComp.dir += 1.0f;
        }

        EntityManager.SetComponentData(Utility.SystemEntity, dataComp);
    }

    public void OnRight(InputAction.CallbackContext context) {
        var dataComp = EntityManager.GetComponentData<InputDataComponent>(Utility.SystemEntity);

        if (context.started) {
            dataComp.state |= InputUtility.right;
            dataComp.dir += 1.0f;
        }

        if (InputUtility.HasState(dataComp, InputUtility.right) && context.canceled) {
            dataComp.state ^= InputUtility.right;
            dataComp.dir += -1.0f;
        }

        EntityManager.SetComponentData(Utility.SystemEntity, dataComp);
    }

    public void OnJump(InputAction.CallbackContext context) {
        var dataComp = EntityManager.GetComponentData<InputDataComponent>(Utility.SystemEntity);

        if (context.started) {
            dataComp.state |= InputUtility.jump;
        }

        if (InputUtility.HasState(dataComp, InputUtility.jump) &&
            context.canceled) {
            dataComp.state ^= InputUtility.jump;
        }

        EntityManager.SetComponentData(Utility.SystemEntity, dataComp);
    }

    public void OnAttack(InputAction.CallbackContext context) {
        var dataComp = EntityManager.GetComponentData<InputDataComponent>(Utility.SystemEntity);

        if (context.started) {
            dataComp.state |= InputUtility.attack;
        }

        if (InputUtility.HasState(dataComp, InputUtility.attack) &&
            context.canceled) {
            dataComp.state ^= InputUtility.attack;
        }

        EntityManager.SetComponentData(Utility.SystemEntity, dataComp);
    }

    public void OnLeftRightAxis(InputAction.CallbackContext context) {
        var dataComp = EntityManager.GetComponentData<InputDataComponent>(Utility.SystemEntity);
        dataComp.dir = context.ReadValue<float>();

        if (false == InputUtility.HasState(dataComp, InputUtility.axis) &&
            context.started) {
            dataComp.state |= InputUtility.axis;
        }

        if (InputUtility.HasState(dataComp, InputUtility.axis) &&
            context.canceled) {
            dataComp.state ^= InputUtility.axis;
        }

        EntityManager.SetComponentData(Utility.SystemEntity, dataComp);
    }

    public void OnCrouch(InputAction.CallbackContext context) {
        var dataComp = EntityManager.GetComponentData<InputDataComponent>(Utility.SystemEntity);
        if (context.started) {
            dataComp.state |= InputUtility.crouch;
        }

        if (context.canceled) {
            dataComp.state ^= InputUtility.crouch;
        }

        EntityManager.SetComponentData(Utility.SystemEntity, dataComp);
    }

    public void OnCrouchAxis(InputAction.CallbackContext context) {
        var dataComp = EntityManager.GetComponentData<InputDataComponent>(Utility.SystemEntity);
        if (-0.5f > context.ReadValue<float>() &&
            false == InputUtility.HasState(dataComp, InputUtility.crouch)) {
            dataComp.state |= InputUtility.crouch;
        }

        if (InputUtility.HasState(dataComp, InputUtility.crouch) &&
            context.canceled) {
            dataComp.state ^= InputUtility.crouch;
        }

        EntityManager.SetComponentData(Utility.SystemEntity, dataComp);
    }

    public void OnConfirm(InputAction.CallbackContext context) {
        if (context.canceled) {
            if (false == EntityManager.HasComponent<ConfirmComponent>(Utility.SystemEntity)) {
                EntityManager.AddComponentData(Utility.SystemEntity, new ConfirmComponent());
            }
        }
    }

    private float _touchDeltaX = 0.0f;
    public void OnTouchLeftRightAxis(InputAction.CallbackContext context) {
        var deltaX = context.ReadValue<float>();
        _touchDeltaX += deltaX * Utility.touchDelta;
        _touchDeltaX = math.clamp(_touchDeltaX, -1.0f, 1.0f);

        var dataComp = EntityManager.GetComponentData<InputDataComponent>(Utility.SystemEntity);
        dataComp.dir = _touchDeltaX;

        EntityManager.SetComponentData(Utility.SystemEntity, dataComp);
    }

    public void OnTouchCrouchAxis(InputAction.CallbackContext context) {
        var deltaY = context.ReadValue<float>();

        var dataComp = EntityManager.GetComponentData<InputDataComponent>(Utility.SystemEntity);
        if (false == InputUtility.HasState(dataComp, InputUtility.crouch) && -10.0f > deltaY) { 
            dataComp.state |= InputUtility.crouch; 
        }
        EntityManager.SetComponentData(Utility.SystemEntity, dataComp);
    }

    public void OnTouch(InputAction.CallbackContext context) {
        if (context.canceled) {
            _touchDeltaX = 0.0f;

            var dataComp = EntityManager.GetComponentData<InputDataComponent>(Utility.SystemEntity);
            dataComp.dir = 0;

            if (InputUtility.HasState(dataComp, InputUtility.crouch)) {
                dataComp.state ^= InputUtility.crouch;
            }
            EntityManager.SetComponentData(Utility.SystemEntity, dataComp);
        }
    }

    private InputActions _input;

    protected override void OnCreate() {
        _input = new InputActions();
        _input.CharacterControl.SetCallbacks(this);
    }

    protected override void OnStartRunning() {
        _input.Enable();
    }

    protected override void OnStopRunning() {
        _input.Disable();
    }

    protected override void OnUpdate() {
    }
}
