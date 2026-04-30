using Sirenix.OdinInspector;
using UnityEngine;

namespace FrameVR.Player.DevFP_VR
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(DevFirstPersonInput))]
    public class DevFirstPersonController : MonoBehaviour
    {
        [Title("Settings")]
        [SerializeField] private DevPlayerSettings settings;

        [Title("References")]
        [SerializeField] private Camera playerCamera;
        [SerializeField] private DevFirstPersonInput input;

        [Title("Debug")]
        [SerializeField, ReadOnly] private bool grounded;
        [SerializeField, ReadOnly] private Vector2 moveInput;
        [SerializeField, ReadOnly] private Vector2 lookInput;
        [SerializeField, ReadOnly] private Vector3 velocity;
        [SerializeField, ReadOnly] private float cameraPitch;

        private CharacterController controller;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();

            if (input == null)
                input = GetComponent<DevFirstPersonInput>();

            if (playerCamera == null)
                playerCamera = GetComponentInChildren<Camera>();

            if (settings == null)
            {
                Debug.LogError("DevFirstPersonController [ERROR] >> No DevPlayerSettings assigned.");
                enabled = false;
                return;
            }

            SetupCharacterController();

            if (settings.lockCursorOnStart)
                LockCursor();
        }

        private void Update()
        {
            if (input == null || settings == null)
                return;

            ReadInput();
            Look();
            Move();
        }

        private void ReadInput()
        {
            moveInput = input.MoveInput;
            lookInput = input.LookInput;
        }

        private void Look()
        {
            float mouseX = lookInput.x * settings.mouseSensitivity;
            float mouseY = lookInput.y * settings.mouseSensitivity;

            transform.Rotate(Vector3.up * mouseX);

            cameraPitch -= mouseY;
            cameraPitch = Mathf.Clamp(cameraPitch, -settings.maxLookAngle, settings.maxLookAngle);

            if (playerCamera != null)
                playerCamera.transform.localEulerAngles = Vector3.right * cameraPitch;
        }

        private void Move()
        {
            grounded = controller.isGrounded;

            if (grounded && velocity.y < 0f)
                velocity.y = -2f;

            Vector3 move =
                transform.right * moveInput.x +
                transform.forward * moveInput.y;

            move = Vector3.ClampMagnitude(move, 1f);

            bool sprinting = input.SprintHeld;
            bool crouching = input.CrouchHeld;
            bool jumpPressed = input.JumpPressed;

            controller.height = crouching
                ? settings.crouchHeight
                : settings.standingHeight;

            float speed = crouching
                ? settings.crouchSpeed
                : sprinting
                    ? settings.sprintSpeed
                    : settings.moveSpeed;

            controller.Move(move * speed * Time.deltaTime);

            if (jumpPressed && grounded)
                velocity.y = Mathf.Sqrt(settings.jumpHeight * -2f * settings.gravity);

            velocity.y += settings.gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }

        private void SetupCharacterController()
        {
            if (controller == null)
                return;

            controller.height = settings.standingHeight;

            Vector3 center = controller.center;
            center.y = settings.standingHeight * 0.5f;
            controller.center = center;
        }

        [Button("Lock Cursor")]
        private void LockCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        [Button("Unlock Cursor")]
        private void UnlockCursor()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (settings == null)
                return;

            if (settings.standingHeight <= 0f)
                settings.standingHeight = 1.8f;

            if (settings.crouchHeight <= 0f)
                settings.crouchHeight = 1.1f;

            if (settings.crouchHeight > settings.standingHeight)
                settings.crouchHeight = settings.standingHeight;

            if (settings.moveSpeed < 0f)
                settings.moveSpeed = 0f;

            if (settings.sprintSpeed < settings.moveSpeed)
                settings.sprintSpeed = settings.moveSpeed;

            if (settings.crouchSpeed < 0f)
                settings.crouchSpeed = 0f;
        }
#endif
    }
}