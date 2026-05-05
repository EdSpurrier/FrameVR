using FrameCoreU.Events;
using FrameVR.Player.Hands;
using HighlightPlus;
using Sirenix.OdinInspector;
using UnityEngine;

namespace FrameVR.Player.Interaction
{
    public class HandInteractable : MonoBehaviour, IHandInteractable
    {
        [FoldoutGroup("Settings")]
        [SerializeField] private bool canBeHeld = true;
        [FoldoutGroup("Settings")]
        [SerializeField] private HighlightEffect highlightEffect;
        
        [BoxGroup("Settings/Throw")]
        [SerializeField] private bool throwOnRelease = true;

        [BoxGroup("Settings/Physics")]
        [SerializeField] private CollisionDetectionMode collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        [BoxGroup("Settings/Physics")]
        [SerializeField] private RigidbodyInterpolation interpolation  = RigidbodyInterpolation.Interpolate;
        
        [FoldoutGroup("Events")]
        [BoxGroup("Events/Grip")]
        [FoldoutGroup("Events/Grip/Grip - Start")]
        [HideLabel]
        public FrameCoreEvent onGripStart = new FrameCoreEvent { eventName = "Grip Start" };
        [FoldoutGroup("Events/Grip/Grip - End")]
        [HideLabel]
        public FrameCoreEvent onGripEnd = new FrameCoreEvent { eventName = "Grip End" };

        [BoxGroup("Events/Trigger")]
        [FoldoutGroup("Events/Trigger/Trigger - Pressed")]
        [HideLabel]
        public FrameCoreEvent onTriggerPressed = new FrameCoreEvent { eventName = "Trigger Pressed" };
        [FoldoutGroup("Events/Trigger/Trigger - Released")]
        [HideLabel]
        public FrameCoreEvent onTriggerReleased = new FrameCoreEvent { eventName = "Trigger Released" };

        [BoxGroup("Events/Primary")]
        
        [FoldoutGroup("Events/Primary/Primary - Pressed")]
        [HideLabel]
        public FrameCoreEvent onPrimaryPressed = new FrameCoreEvent { eventName = "Primary Pressed" };
        [FoldoutGroup("Events/Primary/Primary - Released")]
        [HideLabel]
        public FrameCoreEvent onPrimaryReleased = new FrameCoreEvent { eventName = "Primary Released" };

        [BoxGroup("Events/Secondary")]
        
        [FoldoutGroup("Events/Secondary/Secondary - Pressed")]
        [HideLabel]
        public FrameCoreEvent onSecondaryPressed = new FrameCoreEvent { eventName = "Secondary Pressed" };
        [FoldoutGroup("Events/Secondary/Secondary - Released")]
        [HideLabel]
        public FrameCoreEvent onSecondaryReleased = new FrameCoreEvent { eventName = "Secondary Released" };
        
        [FoldoutGroup("Debug")]
        [SerializeField, ReadOnly] private bool isHeld;
        [FoldoutGroup("Debug")]
        [SerializeField, ReadOnly] private PlayerHand heldBy;

        private Rigidbody rb;
        private bool pullingToHand;
        
        public HighlightEffect HighlightEffect => highlightEffect;
        public bool CanBeHeld => canBeHeld;
        public bool IsHeld => isHeld;
        public PlayerHand HeldBy => heldBy;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            
            if (!rb)
            {
                UnityEngine.Debug.LogError("HandInteractable [ERROR] >> Rigidbody is null.");
                return;
            }

            rb.interpolation = interpolation;
            rb.collisionDetectionMode = collisionDetectionMode;
        }

        public virtual void OnHeld(PlayerHand hand)
        {
            if (!canBeHeld)
                return;

            isHeld = true;
            heldBy = hand;

            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
                rb.interpolation = RigidbodyInterpolation.None;
            }

            if (hand.HoldMode == PlayerHandSettings.HoldAttachMode.PullToHand)
            {
                pullingToHand = true;
                return;
            }

            CompleteHold();
        }
        
        private void Update()
        {
            if (!pullingToHand || heldBy == null)
                return;

            transform.position = Vector3.Lerp(
                transform.position,
                heldBy.transform.position,
                heldBy.PullToHandSpeed * Time.deltaTime
            );

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                heldBy.transform.rotation,
                heldBy.PullToHandSpeed * Time.deltaTime
            );

            if (Vector3.Distance(transform.position, heldBy.transform.position) <= heldBy.PullSnapDistance)
                CompleteHold();
        }
        
        private void CompleteHold()
        {
            pullingToHand = false;

            transform.SetParent(heldBy.transform);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }

        public virtual void OnReleased(PlayerHand hand)
        {
            if (heldBy != hand)
                return;

            pullingToHand = false;

            transform.SetParent(null);

            if (rb != null)
            {
                rb.isKinematic = false;
                rb.interpolation = interpolation;

                if (throwOnRelease && hand.ThrowEnabled)
                {
                    Vector3 releaseVelocity = hand.Velocity * hand.ThrowMultiplier;
                    releaseVelocity = Vector3.ClampMagnitude(releaseVelocity, hand.MaxThrowSpeed);

                    rb.linearVelocity = releaseVelocity;

                    rb.angularVelocity = hand.ApplyAngularVelocity
                        ? hand.AngularVelocity
                        : Vector3.zero;
                }
                else
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }
            }

            isHeld = false;
            heldBy = null;
        }

        public virtual void OnGripStart(PlayerHand hand)
        {
            onGripStart.Activate();
        }

        public virtual void OnGripEnd(PlayerHand hand)
        {
            onGripEnd.Activate();
        }

        public virtual void OnTriggerPressed(PlayerHand hand)
        {
            onTriggerPressed.Activate();
        }

        public virtual void OnTriggerReleased(PlayerHand hand)
        {
            onTriggerReleased.Activate();
        }

        public virtual void OnPrimaryPressed(PlayerHand hand)
        {
            onPrimaryPressed.Activate();
        }

        public virtual void OnPrimaryReleased(PlayerHand hand)
        {
            onPrimaryReleased.Activate();
        }

        public virtual void OnSecondaryPressed(PlayerHand hand)
        {
            onSecondaryPressed.Activate();
        }

        public virtual void OnSecondaryReleased(PlayerHand hand)
        {
            onSecondaryReleased.Activate();
        }
    }
}