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

    private bool _isTouch = false;
    private const float LIMIT_DELTA_X = 3.0f;
    public void OnTouchLeftRightAxis(InputAction.CallbackContext context) {
        var deltaX = context.ReadValue<float>();
        if (false == _isTouch || LIMIT_DELTA_X > math.abs(deltaX)) {
            return;
        }

        var dataComp = EntityManager.GetComponentData<InputDataComponent>(Utility.SystemEntity);
        if (InputUtility.HasState(dataComp, InputUtility.crouch)) {
            return;
        }

        dataComp.dir = math.clamp(deltaX, -1.0f, 1.0f);
        EntityManager.SetComponentData(Utility.SystemEntity, dataComp);
    }

    private const float LIMIT_DELTA_Y = 4.0f;
    public void OnTouchCrouchAxis(InputAction.CallbackContext context) {
        var deltaY = context.ReadValue<float>();
        if (false == _isTouch || LIMIT_DELTA_Y > math.abs(deltaY)) {
            return;
        }

        var dataComp = EntityManager.GetComponentData<InputDataComponent>(Utility.SystemEntity);
        var isCrouch = InputUtility.HasState(dataComp, InputUtility.crouch);
        // if (isCrouch && LIMIT_DELTA_Y < deltaY) {
        //     dataComp.state ^= InputUtility.crouch;
        // }

        if (false == isCrouch && -LIMIT_DELTA_Y > deltaY) {
            dataComp.dir = 0.0f;
            dataComp.state |= InputUtility.crouch;
            EntityManager.SetComponentData(Utility.SystemEntity, dataComp);
        }
    }

    public void OnTouch(InputAction.CallbackContext context) {
        if (context.started) {
            _isTouch = true;
        }

        if (context.canceled) {
            _isTouch = false;

            var dataComp = EntityManager.GetComponentData<InputDataComponent>(Utility.SystemEntity);
            dataComp.dir = 0;
            dataComp.state &= ~InputUtility.crouch;
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
