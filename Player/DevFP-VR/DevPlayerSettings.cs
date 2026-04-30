using Sirenix.OdinInspector;
using UnityEngine;

namespace FrameVR.Player.DevFP_VR
{
    [CreateAssetMenu(menuName = "Game/Player/Dev FP VR Settings")]
    public class DevPlayerSettings : ScriptableObject
    {
        [Title("Look")]
        public float mouseSensitivity = 0.12f;
        public float maxLookAngle = 85f;

        [Title("Movement")]
        public float moveSpeed = 5f;
        public float sprintSpeed = 8f;
        public float gravity = -20f;
        public float jumpHeight = 1.2f;

        [Title("Crouch")]
        public float standingHeight = 1.8f;
        public float crouchHeight = 1.1f;
        public float crouchSpeed = 2.5f;

        [Title("Cursor")]
        public bool lockCursorOnStart = true;
    }
}