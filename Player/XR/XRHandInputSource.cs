using FrameVR.Player.Hands;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FrameVR.Player.XR
{
    public class XRHandInputSource : MonoBehaviour, IHandInputSource
    {
        [Title("Input Asset")]
        [SerializeField] private InputActionAsset inputActions;
        [SerializeField] private string actionMapName = "XRI LeftHand";

        [Title("Action Names")]
        [SerializeField] private string gripActionName = "Select Value";
        [SerializeField] private string triggerActionName = "Activate Value";
        [SerializeField] private string primaryActionName = "Primary Button";
        [SerializeField] private string secondaryActionName = "Secondary Button";

        [Title("Debug")]
        [SerializeField, ReadOnly] private float gripValue;
        [SerializeField, ReadOnly] private float triggerValue;
        [SerializeField, ReadOnly] private bool primaryPressed;
        [SerializeField, ReadOnly] private bool secondaryPressed;

        public float Grip => gripValue;
        public float Trigger => triggerValue;
        public bool PrimaryPressed => primaryPressed;
        public bool SecondaryPressed => secondaryPressed;

        private InputActionMap actionMap;
        private InputAction grip;
        private InputAction trigger;
        private InputAction primary;
        private InputAction secondary;

        private void Awake()
        {
            BindActions();
        }

        private void OnEnable()
        {
            actionMap?.Enable();
        }

        private void OnDisable()
        {
            actionMap?.Disable();
        }

        private void Update()
        {
            gripValue = grip != null ? grip.ReadValue<float>() : 0f;
            triggerValue = trigger != null ? trigger.ReadValue<float>() : 0f;

            primaryPressed = primary != null && primary.IsPressed();
            secondaryPressed = secondary != null && secondary.IsPressed();
        }

        private void BindActions()
        {
            if (inputActions == null)
            {
                Debug.LogError("XRHandInputSource [ERROR] >> No InputActionAsset assigned.");
                return;
            }

            actionMap = inputActions.FindActionMap(actionMapName);

            if (actionMap == null)
            {
                Debug.LogError($"XRHandInputSource [ERROR] >> No action map found named '{actionMapName}'.");
                return;
            }

            grip = actionMap.FindAction(gripActionName);
            trigger = actionMap.FindAction(triggerActionName);
            primary = actionMap.FindAction(primaryActionName);
            secondary = actionMap.FindAction(secondaryActionName);

            ValidateAction(grip, gripActionName);
            ValidateAction(trigger, triggerActionName);
            ValidateAction(primary, primaryActionName);
            ValidateAction(secondary, secondaryActionName);
        }

        private void ValidateAction(InputAction action, string actionName)
        {
            if (action == null)
                Debug.LogWarning($"XRHandInputSource [WARNING] >> Missing action '{actionName}'.");
        }
    }
}