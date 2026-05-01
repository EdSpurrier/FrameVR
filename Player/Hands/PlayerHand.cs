using System.Collections.Generic;
using FrameVR.Player.Interaction;
using Sirenix.OdinInspector;
using UnityEngine;

namespace FrameVR.Player.Hands
{
    public class PlayerHand : MonoBehaviour
    {
        [Title("Held Object")]
        [SerializeField, ReadOnly] private MonoBehaviour heldInteractableBehaviour;

        private IHandInteractable heldInteractable;

        public IHandInteractable HeldInteractable => heldInteractable;
        public bool HasHeldObject => heldInteractable != null;
        
        [Title("Input Source")]
        [SerializeField] private MonoBehaviour inputSourceBehaviour;
        
        private IHandInputSource inputSource;

        [Title("Settings")]
        [SerializeField] private PlayerHandSettings settings;

        public PlayerHandSettings Settings => settings;
        public bool GripIsToggle => settings != null && settings.gripMode == PlayerHandSettings.GripMode.Toggle;
        
        public PlayerHandSettings.HoldAttachMode HoldMode =>
            settings != null
                ? settings.holdAttachMode
                : PlayerHandSettings.HoldAttachMode.SnapInstant;

        public float PullToHandSpeed =>
            settings != null ? settings.pullToHandSpeed : 12f;

        public float PullSnapDistance =>
            settings != null ? settings.pullSnapDistance : 0.03f;
        
        [Title("Velocity")]
        [SerializeField, ReadOnly] private Vector3 velocity;
        [SerializeField, ReadOnly] private Vector3 angularVelocity;

        private Vector3 lastPosition;
        private Quaternion lastRotation;

        public Vector3 Velocity => velocity;
        public Vector3 AngularVelocity => angularVelocity;

        [Title("Throw")]
        public bool ThrowEnabled => settings != null && settings.enableThrow;

        public int VelocitySampleFrames =>
            settings != null ? settings.velocitySampleFrames : 6;

        public float ThrowMultiplier =>
            settings != null ? settings.throwMultiplier : 1.2f;

        public float MaxThrowSpeed =>
            settings != null ? settings.maxThrowSpeed : 10f;

        public bool ApplyAngularVelocity =>
            settings != null && settings.applyAngularVelocity;
        
        private readonly Queue<Vector3> velocitySamples = new();
        
        [Title("Pickup")]
        public PlayerHandSettings.PickupMode PickupMode =>
            settings != null
                ? settings.pickupMode
                : PlayerHandSettings.PickupMode.NearAndFar;

        public LayerMask PickupMask =>
            settings != null ? settings.pickupMask : ~0;

        public float RayDistance =>
            settings != null ? settings.rayDistance : 4f;

        public float OverlapRadius =>
            settings != null ? settings.overlapRadius : 0.25f;

        public bool ReleaseOnGripEnd =>
            settings == null || settings.releaseOnGripEnd;
        
        
        [Title("Debug")]
        [ShowInInspector, ReadOnly] public float Grip { get; private set; }
        [ShowInInspector, ReadOnly] public float Trigger { get; private set; }
        [ShowInInspector, ReadOnly] public bool PrimaryPressed { get; private set; }
        [ShowInInspector, ReadOnly] public bool SecondaryPressed { get; private set; }

        public bool TryHold(MonoBehaviour interactableBehaviour)
        {
            if (interactableBehaviour == null)
                return false;

            IHandInteractable interactable = interactableBehaviour as IHandInteractable;

            if (interactable == null || !interactable.CanBeHeld)
                return false;

            if (heldInteractable != null)
                ReleaseHeldObject();

            heldInteractable = interactable;
            heldInteractableBehaviour = interactableBehaviour;

            heldInteractable.OnHeld(this);
            return true;
        }

        public void ReleaseHeldObject()
        {
            if (heldInteractable == null)
                return;

            heldInteractable.OnReleased(this);

            heldInteractable = null;
            heldInteractableBehaviour = null;
        }
        
        private void Awake()
        {
            inputSource = inputSourceBehaviour as IHandInputSource;

            if (inputSourceBehaviour != null && inputSource == null)
            {
                Debug.LogError($"{inputSourceBehaviour.name} does not implement IHandInputSource.");
            }
            
            lastPosition = transform.position;
            lastRotation = transform.rotation;
        }

        private void Update()
        {
            if (inputSource == null)
                return;

            UpdateVelocity();
            
            Grip = inputSource.Grip;
            Trigger = inputSource.Trigger;
            PrimaryPressed = inputSource.PrimaryPressed;
            SecondaryPressed = inputSource.SecondaryPressed;
        }
        
        private void UpdateVelocity()
        {
            float deltaTime = Time.deltaTime;
            if (deltaTime <= 0f)
                return;

            Vector3 frameVelocity = (transform.position - lastPosition) / deltaTime;

            velocitySamples.Enqueue(frameVelocity);

            while (velocitySamples.Count > VelocitySampleFrames)
                velocitySamples.Dequeue();

            velocity = GetAverageVelocity();

            lastPosition = transform.position;
        }

        private Vector3 GetAverageVelocity()
        {
            if (velocitySamples.Count == 0)
                return Vector3.zero;

            Vector3 total = Vector3.zero;

            foreach (Vector3 sample in velocitySamples)
                total += sample;

            return total / velocitySamples.Count;
        }
    }
}