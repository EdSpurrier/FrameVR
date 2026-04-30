using FrameVR.Player.Hands;
using Sirenix.OdinInspector;
using UnityEngine;

namespace FrameVR.Player.Interaction
{
    public class HandInteractable : MonoBehaviour, IHandInteractable
    {
        [Title("Settings")]
        [SerializeField] private bool canBeHeld = true;

        [Title("Debug")]
        [SerializeField, ReadOnly] private bool isHeld;
        [SerializeField, ReadOnly] private PlayerHand heldBy;

        private Rigidbody rb;
        private bool pullingToHand;
        
        public bool CanBeHeld => canBeHeld;
        public bool IsHeld => isHeld;
        public PlayerHand HeldBy => heldBy;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
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
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            isHeld = false;
            heldBy = null;
        }

        public virtual void OnGripStart(PlayerHand hand) { }
        public virtual void OnGripEnd(PlayerHand hand) { }
        public virtual void OnTriggerPressed(PlayerHand hand) { }
        public virtual void OnTriggerReleased(PlayerHand hand) { }
        public virtual void OnPrimaryPressed(PlayerHand hand) { }
        public virtual void OnPrimaryReleased(PlayerHand hand) { }
        public virtual void OnSecondaryPressed(PlayerHand hand) { }
        public virtual void OnSecondaryReleased(PlayerHand hand) { }
    }
}