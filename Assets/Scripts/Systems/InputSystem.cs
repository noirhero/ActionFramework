// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Systems;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

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
    private float2 _touchPos = new float2();
    private const float LIMIT_DELTA_Y = 6.0f;
    public void OnTouch(InputAction.CallbackContext context) {
        var touchState = context.ReadValue<TouchState>();
        switch (touchState.phase) {
            case TouchPhase.Began:
                _isTouch = true;
                _touchPos = touchState.position;
                break;
            case TouchPhase.Ended: {
                _isTouch = false;

                var dataComp = EntityManager.GetComponentData<InputDataComponent>(Utility.SystemEntity);
                dataComp.dir = 0;
                dataComp.state &= ~InputUtility.crouch;
                EntityManager.SetComponentData(Utility.SystemEntity, dataComp);
                break;
            }
        }
    }

    public void OnTouchAxis(InputAction.CallbackContext context) {
        if (false == _isTouch) {
            return;
        }

        var touchState = context.ReadValue<TouchState>();
        var delta = new float2(touchState.position) - _touchPos;
        _touchPos = touchState.position;

        var deltaX = math.abs(delta.x);
        var deltaY = math.abs(delta.y);
        if (deltaX > deltaY) {
            var dataComp = EntityManager.GetComponentData<InputDataComponent>(Utility.SystemEntity);
            dataComp.dir = math.normalizesafe(new float2(delta.x, 0.0f)).x;
            dataComp.state &= ~InputUtility.crouch;
            EntityManager.SetComponentData(Utility.SystemEntity, dataComp);
        }
        else if (deltaX < deltaY && LIMIT_DELTA_Y > delta.y) {
            var dataComp = EntityManager.GetComponentData<InputDataComponent>(Utility.SystemEntity);
            dataComp.state |= InputUtility.crouch;
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
