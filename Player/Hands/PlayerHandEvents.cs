using FrameCoreU.Events;
using Sirenix.OdinInspector;
using UnityEngine;

namespace FrameVR.Player.Hands
{
    public class PlayerHandEvents : MonoBehaviour
    {
        [Title("Reference")]
        [SerializeField] private PlayerHand hand;

        [Title("Grip")]
        public FrameCoreEvent onGripStart = new FrameCoreEvent { eventName = "Grip Start" };
        public FrameCoreEvent onGripEnd = new FrameCoreEvent { eventName = "Grip End" };

        [Title("Trigger")]
        public FrameCoreEvent onTriggerPressed = new FrameCoreEvent { eventName = "Trigger Pressed" };
        public FrameCoreEvent onTriggerReleased = new FrameCoreEvent { eventName = "Trigger Released" };

        [Title("Primary")]
        public FrameCoreEvent onPrimaryPressed = new FrameCoreEvent { eventName = "Primary Pressed" };
        public FrameCoreEvent onPrimaryReleased = new FrameCoreEvent { eventName = "Primary Released" };

        [Title("Secondary")]
        public FrameCoreEvent onSecondaryPressed = new FrameCoreEvent { eventName = "Secondary Pressed" };
        public FrameCoreEvent onSecondaryReleased = new FrameCoreEvent { eventName = "Secondary Released" };

        private float lastGrip;
        private float lastTrigger;
        private bool lastPrimary;
        private bool lastSecondary;

        private const float threshold = 0.1f;

        private bool toggleGripActive;
        
        private void Awake()
        {
            if (hand == null)
                hand = GetComponent<PlayerHand>();
        }

        private void Update()
        {
            if (hand == null)
                return;

            CheckGrip();
            CheckTrigger();
            CheckButtons();
        }

        
        private void CheckGrip()
        {
            bool was = lastGrip > threshold;
            bool now = hand.Grip > threshold;

            if (hand.GripIsToggle)
            {
                if (!was && now)
                {
                    toggleGripActive = !toggleGripActive;

                    if (toggleGripActive)
                    {
                        onGripStart.Activate();
                        hand.HeldInteractable?.OnGripStart(hand);
                    }
                    else
                    {
                        onGripEnd.Activate();
                        hand.HeldInteractable?.OnGripEnd(hand);

                        if (hand.ReleaseOnGripEnd && hand.TryGetComponent(out PlayerHandPickup pickup))
                            pickup.Release();
                    }
                }
            }
            else
            {
                if (!was && now)
                    onGripStart.Activate();

                if (was && !now)
                {
                    onGripEnd.Activate();
                    hand.HeldInteractable?.OnGripEnd(hand);

                    if (hand.ReleaseOnGripEnd && hand.TryGetComponent(out PlayerHandPickup pickup))
                        pickup.Release();
                }
            }

            lastGrip = hand.Grip;
        }

        private void CheckTrigger()
        {
            bool was = lastTrigger > threshold;
            bool now = hand.Trigger > threshold;

            if (!was && now)
            {
                onTriggerPressed.Activate();
                hand.HeldInteractable?.OnTriggerPressed(hand);
            }

            if (was && !now)
            {
                onTriggerReleased.Activate();
                hand.HeldInteractable?.OnTriggerReleased(hand);
            }

            lastTrigger = hand.Trigger;
        }

        private void CheckButtons()
        {
            if (!lastPrimary && hand.PrimaryPressed)
            {
                onPrimaryPressed.Activate();
                hand.HeldInteractable?.OnPrimaryPressed(hand);
            }

            if (lastPrimary && !hand.PrimaryPressed)
            {
                onPrimaryReleased.Activate();
                hand.HeldInteractable?.OnPrimaryReleased(hand);
            }

            if (!lastSecondary && hand.SecondaryPressed)
            {
                onSecondaryPressed.Activate();
                hand.HeldInteractable?.OnSecondaryPressed(hand);
            }

            if (lastSecondary && !hand.SecondaryPressed)
            {
                onSecondaryReleased.Activate();
                hand.HeldInteractable?.OnSecondaryReleased(hand);
            }

            lastPrimary = hand.PrimaryPressed;
            lastSecondary = hand.SecondaryPressed;
        }
    }
}