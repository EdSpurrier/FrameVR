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
        }

        private void Update()
        {
            if (inputSource == null)
                return;

            Grip = inputSource.Grip;
            Trigger = inputSource.Trigger;
            PrimaryPressed = inputSource.PrimaryPressed;
            SecondaryPressed = inputSource.SecondaryPressed;
        }
    }
}