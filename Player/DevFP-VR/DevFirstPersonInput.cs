using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FrameVR.Player.DevFP_VR
{
    public class DevFirstPersonInput : MonoBehaviour
    {
        [Title("Input Asset")]
        [SerializeField] private InputActionAsset inputActions;
        [SerializeField] private string actionMapName = "Player";

        [Title("Action Names")]
        [SerializeField] private string moveActionName = "Move";
        [SerializeField] private string lookActionName = "Look";
        [SerializeField] private string jumpActionName = "Jump";
        [SerializeField] private string sprintActionName = "Sprint";
        [SerializeField] private string crouchActionName = "Crouch";

        [Title("Debug")]
        [SerializeField, ReadOnly] private Vector2 moveInput;
        [SerializeField, ReadOnly] private Vector2 lookInput;
        [SerializeField, ReadOnly] private bool jumpPressed;
        [SerializeField, ReadOnly] private bool sprintHeld;
        [SerializeField, ReadOnly] private bool crouchHeld;

        public Vector2 MoveInput => moveInput;
        public Vector2 LookInput => lookInput;
        public bool JumpPressed => jumpPressed;
        public bool SprintHeld => sprintHeld;
        public bool CrouchHeld => crouchHeld;

        private InputActionMap actionMap;
        private InputAction move;
        private InputAction look;
        private InputAction jump;
        private InputAction sprint;
        private InputAction crouch;

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
            moveInput = move != null ? move.ReadValue<Vector2>() : Vector2.zero;
            lookInput = look != null ? look.ReadValue<Vector2>() : Vector2.zero;

            jumpPressed = jump != null && jump.WasPressedThisFrame();
            sprintHeld = sprint != null && sprint.IsPressed();
            crouchHeld = crouch != null && crouch.IsPressed();
        }

        private void BindActions()
        {
            if (inputActions == null)
            {
                Debug.LogError("DevFirstPersonInput [ERROR] >> No InputActionAsset assigned.");
                return;
            }

            actionMap = inputActions.FindActionMap(actionMapName);

            if (actionMap == null)
            {
                Debug.LogError($"DevFirstPersonInput [ERROR] >> No action map found named '{actionMapName}'.");
                return;
            }

            move = actionMap.FindAction(moveActionName);
            look = actionMap.FindAction(lookActionName);
            jump = actionMap.FindAction(jumpActionName);
            sprint = actionMap.FindAction(sprintActionName);
            crouch = actionMap.FindAction(crouchActionName);

            ValidateAction(move, moveActionName);
            ValidateAction(look, lookActionName);
            ValidateAction(jump, jumpActionName);
            ValidateAction(sprint, sprintActionName);
            ValidateAction(crouch, crouchActionName);
        }

        private void ValidateAction(InputAction action, string actionName)
        {
            if (action == null)
                Debug.LogWarning($"DevFirstPersonInput [WARNING] >> Missing action '{actionName}'.");
        }
    }
}