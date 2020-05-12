// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestInputSystem : ComponentSystem, InputActions.ICharacterControlActions {
    public void OnLeft(InputAction.CallbackContext context) {
        if (context.started) {
            Debug.Log("Left key start.");
        }

        if (context.canceled) {
            Debug.Log("Left key end");
        }
    }

    public void OnRight(InputAction.CallbackContext context) {
        if (context.started) {
            Debug.Log("Right key start.");
        }

        if (context.canceled) {
            Debug.Log("Right key end");
        }
    }

    public void OnJump(InputAction.CallbackContext context) {
        if (context.started) {
            Debug.Log("Jump key start.");
        }

        if (context.canceled) {
            Debug.Log("Jump key end");
        }
    }

    public void OnAttack(InputAction.CallbackContext context) {
        if (context.started) {
            Debug.Log("Attack key start.");
        }

        if (context.canceled) {
            Debug.Log("Attack key end");
        }
    }

    public void OnLeftRightAxis(InputAction.CallbackContext context) {
        Debug.Log($"Stick axis value : {context.ReadValue<float>()}");
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