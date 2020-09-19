// GENERATED AUTOMATICALLY FROM 'Assets/Input/InputActions.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @InputActions : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @InputActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputActions"",
    ""maps"": [
        {
            ""name"": ""CharacterControl"",
            ""id"": ""3713082f-bb36-41b6-8a52-552522de85bd"",
            ""actions"": [
                {
                    ""name"": ""Left"",
                    ""type"": ""Button"",
                    ""id"": ""9eb62d50-1ddc-47b2-8af0-e9bc0f4fb0cf"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Right"",
                    ""type"": ""Button"",
                    ""id"": ""c48bbed3-6950-4332-ae0d-e9ee4d0058b0"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""22ff7d96-0edf-4568-b25f-d44e48cb5c6a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Attack"",
                    ""type"": ""Button"",
                    ""id"": ""d3037a47-d5bc-47eb-8e17-a8a5212aaec7"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""LeftRightAxis"",
                    ""type"": ""Value"",
                    ""id"": ""556d4730-75b1-47ec-8457-647da7614bcb"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Crouch"",
                    ""type"": ""Button"",
                    ""id"": ""9d15fe7d-9964-4876-bb11-8630d83d6b67"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""CrouchAxis"",
                    ""type"": ""Value"",
                    ""id"": ""52f9a910-9a8d-4642-80aa-d64e1918fa3f"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Confirm"",
                    ""type"": ""Button"",
                    ""id"": ""85ffcda8-4190-4309-b597-492d8b1fb77c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""2d40ba2a-4a9f-44b0-acf5-5f95bb5ed7e1"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Left"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e0e5675f-ac65-4702-ae42-727446cc389c"",
                    ""path"": ""<Gamepad>/dpad/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Left"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""80583f76-abb7-4d11-b63d-f1c16c9e0d2a"",
                    ""path"": ""<Touchscreen>/primaryTouch/indirectTouch"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Left"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3a27bef1-8584-4bdd-a4c4-a36b8a7a9204"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Right"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ba80465a-514d-4806-af20-5ccb7830882f"",
                    ""path"": ""<Gamepad>/dpad/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Right"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""89229752-d95e-4296-a6d0-1a5da74fc599"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1d530ba4-e0ef-42b6-8621-a63163cfe32e"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""bab4cb38-1ce8-4d11-bb0d-5e88cc2569f1"",
                    ""path"": ""<Keyboard>/ctrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""22bf601c-0f89-4ccf-b3da-a2fe3176fa91"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""051f2a69-5a2e-43ed-9376-7eff2213bd90"",
                    ""path"": ""<Gamepad>/leftStick/x"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LeftRightAxis"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3617dcae-87f9-415c-aa45-0152a75b4e45"",
                    ""path"": ""<Touchscreen>/delta/x"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LeftRightAxis"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c584780c-14a0-4ad8-9656-0c1502b81d4e"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Crouch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""67696cf3-61a9-44d6-bfa2-1fe65b79f1d4"",
                    ""path"": ""<Gamepad>/dpad/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Crouch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""96ff6078-0673-4a3a-b3df-2f3264fb9901"",
                    ""path"": ""<Gamepad>/leftStick/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CrouchAxis"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8def72a3-55a7-4593-ab16-eb824529f94a"",
                    ""path"": ""<Touchscreen>/delta/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CrouchAxis"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5fa3d4c6-0e4c-4d06-922c-55ebc35eeab2"",
                    ""path"": ""<Keyboard>/enter"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Confirm"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5847d20a-48dc-4880-934c-e296c02f51c4"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Confirm"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""66d30ff4-8bff-4d40-81c5-a4bed0291d45"",
                    ""path"": ""<Gamepad>/select"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Confirm"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // CharacterControl
        m_CharacterControl = asset.FindActionMap("CharacterControl", throwIfNotFound: true);
        m_CharacterControl_Left = m_CharacterControl.FindAction("Left", throwIfNotFound: true);
        m_CharacterControl_Right = m_CharacterControl.FindAction("Right", throwIfNotFound: true);
        m_CharacterControl_Jump = m_CharacterControl.FindAction("Jump", throwIfNotFound: true);
        m_CharacterControl_Attack = m_CharacterControl.FindAction("Attack", throwIfNotFound: true);
        m_CharacterControl_LeftRightAxis = m_CharacterControl.FindAction("LeftRightAxis", throwIfNotFound: true);
        m_CharacterControl_Crouch = m_CharacterControl.FindAction("Crouch", throwIfNotFound: true);
        m_CharacterControl_CrouchAxis = m_CharacterControl.FindAction("CrouchAxis", throwIfNotFound: true);
        m_CharacterControl_Confirm = m_CharacterControl.FindAction("Confirm", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // CharacterControl
    private readonly InputActionMap m_CharacterControl;
    private ICharacterControlActions m_CharacterControlActionsCallbackInterface;
    private readonly InputAction m_CharacterControl_Left;
    private readonly InputAction m_CharacterControl_Right;
    private readonly InputAction m_CharacterControl_Jump;
    private readonly InputAction m_CharacterControl_Attack;
    private readonly InputAction m_CharacterControl_LeftRightAxis;
    private readonly InputAction m_CharacterControl_Crouch;
    private readonly InputAction m_CharacterControl_CrouchAxis;
    private readonly InputAction m_CharacterControl_Confirm;
    public struct CharacterControlActions
    {
        private @InputActions m_Wrapper;
        public CharacterControlActions(@InputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Left => m_Wrapper.m_CharacterControl_Left;
        public InputAction @Right => m_Wrapper.m_CharacterControl_Right;
        public InputAction @Jump => m_Wrapper.m_CharacterControl_Jump;
        public InputAction @Attack => m_Wrapper.m_CharacterControl_Attack;
        public InputAction @LeftRightAxis => m_Wrapper.m_CharacterControl_LeftRightAxis;
        public InputAction @Crouch => m_Wrapper.m_CharacterControl_Crouch;
        public InputAction @CrouchAxis => m_Wrapper.m_CharacterControl_CrouchAxis;
        public InputAction @Confirm => m_Wrapper.m_CharacterControl_Confirm;
        public InputActionMap Get() { return m_Wrapper.m_CharacterControl; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(CharacterControlActions set) { return set.Get(); }
        public void SetCallbacks(ICharacterControlActions instance)
        {
            if (m_Wrapper.m_CharacterControlActionsCallbackInterface != null)
            {
                @Left.started -= m_Wrapper.m_CharacterControlActionsCallbackInterface.OnLeft;
                @Left.performed -= m_Wrapper.m_CharacterControlActionsCallbackInterface.OnLeft;
                @Left.canceled -= m_Wrapper.m_CharacterControlActionsCallbackInterface.OnLeft;
                @Right.started -= m_Wrapper.m_CharacterControlActionsCallbackInterface.OnRight;
                @Right.performed -= m_Wrapper.m_CharacterControlActionsCallbackInterface.OnRight;
                @Right.canceled -= m_Wrapper.m_CharacterControlActionsCallbackInterface.OnRight;
                @Jump.started -= m_Wrapper.m_CharacterControlActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_CharacterControlActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_CharacterControlActionsCallbackInterface.OnJump;
                @Attack.started -= m_Wrapper.m_CharacterControlActionsCallbackInterface.OnAttack;
                @Attack.performed -= m_Wrapper.m_CharacterControlActionsCallbackInterface.OnAttack;
                @Attack.canceled -= m_Wrapper.m_CharacterControlActionsCallbackInterface.OnAttack;
                @LeftRightAxis.started -= m_Wrapper.m_CharacterControlActionsCallbackInterface.OnLeftRightAxis;
                @LeftRightAxis.performed -= m_Wrapper.m_CharacterControlActionsCallbackInterface.OnLeftRightAxis;
                @LeftRightAxis.canceled -= m_Wrapper.m_CharacterControlActionsCallbackInterface.OnLeftRightAxis;
                @Crouch.started -= m_Wrapper.m_CharacterControlActionsCallbackInterface.OnCrouch;
                @Crouch.performed -= m_Wrapper.m_CharacterControlActionsCallbackInterface.OnCrouch;
                @Crouch.canceled -= m_Wrapper.m_CharacterControlActionsCallbackInterface.OnCrouch;
                @CrouchAxis.started -= m_Wrapper.m_CharacterControlActionsCallbackInterface.OnCrouchAxis;
                @CrouchAxis.performed -= m_Wrapper.m_CharacterControlActionsCallbackInterface.OnCrouchAxis;
                @CrouchAxis.canceled -= m_Wrapper.m_CharacterControlActionsCallbackInterface.OnCrouchAxis;
                @Confirm.started -= m_Wrapper.m_CharacterControlActionsCallbackInterface.OnConfirm;
                @Confirm.performed -= m_Wrapper.m_CharacterControlActionsCallbackInterface.OnConfirm;
                @Confirm.canceled -= m_Wrapper.m_CharacterControlActionsCallbackInterface.OnConfirm;
            }
            m_Wrapper.m_CharacterControlActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Left.started += instance.OnLeft;
                @Left.performed += instance.OnLeft;
                @Left.canceled += instance.OnLeft;
                @Right.started += instance.OnRight;
                @Right.performed += instance.OnRight;
                @Right.canceled += instance.OnRight;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @Attack.started += instance.OnAttack;
                @Attack.performed += instance.OnAttack;
                @Attack.canceled += instance.OnAttack;
                @LeftRightAxis.started += instance.OnLeftRightAxis;
                @LeftRightAxis.performed += instance.OnLeftRightAxis;
                @LeftRightAxis.canceled += instance.OnLeftRightAxis;
                @Crouch.started += instance.OnCrouch;
                @Crouch.performed += instance.OnCrouch;
                @Crouch.canceled += instance.OnCrouch;
                @CrouchAxis.started += instance.OnCrouchAxis;
                @CrouchAxis.performed += instance.OnCrouchAxis;
                @CrouchAxis.canceled += instance.OnCrouchAxis;
                @Confirm.started += instance.OnConfirm;
                @Confirm.performed += instance.OnConfirm;
                @Confirm.canceled += instance.OnConfirm;
            }
        }
    }
    public CharacterControlActions @CharacterControl => new CharacterControlActions(this);
    public interface ICharacterControlActions
    {
        void OnLeft(InputAction.CallbackContext context);
        void OnRight(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnAttack(InputAction.CallbackContext context);
        void OnLeftRightAxis(InputAction.CallbackContext context);
        void OnCrouch(InputAction.CallbackContext context);
        void OnCrouchAxis(InputAction.CallbackContext context);
        void OnConfirm(InputAction.CallbackContext context);
    }
}
