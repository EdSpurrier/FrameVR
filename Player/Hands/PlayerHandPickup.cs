using FrameVR.Player.Interaction;
using Sirenix.OdinInspector;
using UnityEngine;

namespace FrameVR.Player.Hands
{
    public class PlayerHandPickup : MonoBehaviour
    {
        [Title("References")]
        [SerializeField] private PlayerHand hand;
        [SerializeField] private Transform rayOrigin;

        [Title("Pickup")]
        private PlayerHandSettings.PickupMode PickupMode =>
            hand != null
                ? hand.PickupMode
                : PlayerHandSettings.PickupMode.NearAndFar;
        
        [Title("Debug")]
        [SerializeField] private bool drawRayInGameView = true;
        [SerializeField] private Color rayColor = Color.green;
        [SerializeField] private Color rayHitColor = Color.red;
        [SerializeField, ReadOnly] private MonoBehaviour currentTarget;

        private readonly Collider[] overlapResults = new Collider[16];

        private void Awake()
        {
            if (hand == null)
                hand = GetComponent<PlayerHand>();

            if (rayOrigin == null)
                rayOrigin = transform;
        }

        public void TryPickup()
        {
            GetCurrentTarget();

            if (currentTarget != null)
                if (hand.TryHold(currentTarget))
                {
                    (currentTarget as IHandInteractable).OnHoverEnd(hand);
                }
        }

        public void GetCurrentTarget()
        {
            if (hand == null)
                return;

            if (hand.HasHeldObject)
                return;

            MonoBehaviour target = PickupMode switch
            {
                PlayerHandSettings.PickupMode.Near => FindNearTarget(),
                PlayerHandSettings.PickupMode.Far => FindFarTarget(),
                PlayerHandSettings.PickupMode.NearAndFar => FindNearTarget() ?? FindFarTarget(),
                _ => null
            };

            if (currentTarget && currentTarget != target)
            {
                (currentTarget as IHandInteractable).OnHoverEnd(hand);
            }
            if (target && currentTarget != target)
            {
                (target as IHandInteractable).OnHoverStart(hand);
            }
            
            currentTarget = target;
        }

        public void Release()
        {
            if (hand == null)
                return;

            hand.ReleaseHeldObject();
        }
        
        private void Update()
        {
            if (!drawRayInGameView)
                return;

            if (PickupMode == PlayerHandSettings.PickupMode.Far ||
                PickupMode == PlayerHandSettings.PickupMode.NearAndFar)
                DrawRayDebug();

            if (PickupMode == PlayerHandSettings.PickupMode.Near ||
                PickupMode == PlayerHandSettings.PickupMode.NearAndFar)
                DrawOverlapDebug();

            GetCurrentTarget();
        }
        
        private void DrawRayDebug()
        {
            if (rayOrigin == null)
                return;

            bool hit = Physics.Raycast(rayOrigin.position, rayOrigin.forward, out RaycastHit hitInfo, hand.RayDistance, hand.PickupMask);

            Debug.DrawRay(
                rayOrigin.position,
                rayOrigin.forward * hand.RayDistance,
                hit ? rayHitColor : rayColor,
                0f
            );
        }
        
        private void DrawOverlapDebug()
        {
            DrawWireSphere(transform.position, hand.OverlapRadius, Color.cyan);
        }
        
        private MonoBehaviour FindFarTarget()
        {
            if (rayOrigin == null)
                return null;

            Vector3 origin = rayOrigin.position;
            Vector3 direction = rayOrigin.forward;
            float distance = hand.RayDistance;
            LayerMask mask = hand.PickupMask;

            // 1. Precision raycast first
            if (Physics.Raycast(origin, direction, out RaycastHit rayHit, distance, mask))
            {
                if (drawRayInGameView)
                    Debug.DrawRay(origin, direction * distance, rayHitColor, 0f);

                MonoBehaviour target = FindInteractable(rayHit.collider);

                if (target != null)
                    return target;
            }

            // 2. Capsule assist second
            float capsuleRadius = hand.FarPickupCapsuleRadius;

            Vector3 capsuleStart = origin;
            Vector3 capsuleEnd = origin + direction * distance;

            Collider[] hits = Physics.OverlapCapsule(
                capsuleStart,
                capsuleEnd,
                capsuleRadius,
                mask
            );

            MonoBehaviour bestTarget = null;
            float bestScore = float.MaxValue;

            foreach (Collider col in hits)
            {
                MonoBehaviour target = FindInteractable(col);

                if (target == null)
                    continue;

                Vector3 toTarget = target.transform.position - origin;

                float angle = Vector3.Angle(direction, toTarget.normalized);
                float targetDistance = toTarget.magnitude;

                // Lower score = better.
                // Angle matters more than distance so it favours what you're pointing at.
                float score = angle * 2f + targetDistance;

                if (score < bestScore)
                {
                    bestScore = score;
                    bestTarget = target;
                }
            }

            if (drawRayInGameView)
            {
                Debug.DrawRay(
                    origin,
                    direction * distance,
                    bestTarget != null ? rayHitColor : rayColor,
                    0f
                );
            }

            return bestTarget;
        }

        private MonoBehaviour FindNearTarget()
        {
            int count = Physics.OverlapSphereNonAlloc(
                transform.position,
                hand.OverlapRadius,
                overlapResults,
                hand.PickupMask
            );

            MonoBehaviour bestTarget = null;
            float bestDistance = float.MaxValue;

            for (int i = 0; i < count; i++)
            {
                Collider col = overlapResults[i];

                if (col == null)
                    continue;

                MonoBehaviour target = FindInteractable(col);

                if (target == null)
                    continue;

                float distance = Vector3.Distance(transform.position, target.transform.position);

                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestTarget = target;
                }
            }

            return bestTarget;
        }

        private MonoBehaviour FindInteractable(Collider col)
        {
            if (col == null)
                return null;

            MonoBehaviour[] behaviours = col.GetComponentsInParent<MonoBehaviour>();

            foreach (MonoBehaviour behaviour in behaviours)
            {
                if (behaviour is IHandInteractable interactable && interactable.CanBeHeld)
                    return behaviour;
            }

            return null;
        }

        private void OnDrawGizmosSelected()
        {
            if ((PickupMode == PlayerHandSettings.PickupMode.Far ||
                    PickupMode == PlayerHandSettings.PickupMode.NearAndFar) && rayOrigin != null)
                Gizmos.DrawRay(rayOrigin.position, rayOrigin.forward * hand.RayDistance);

            if (PickupMode == PlayerHandSettings.PickupMode.Near ||
                PickupMode == PlayerHandSettings.PickupMode.NearAndFar)
                Gizmos.DrawWireSphere(transform.position, hand.OverlapRadius);
        }
        
        private void DrawWireSphere(Vector3 center, float radius, Color color)
        {
            const int segments = 24;

            float angleStep = 360f / segments;

            // XY circle
            for (int i = 0; i < segments; i++)
            {
                float a = Mathf.Deg2Rad * (i * angleStep);
                float b = Mathf.Deg2Rad * ((i + 1) * angleStep);

                Vector3 p1 = center + new Vector3(Mathf.Cos(a), Mathf.Sin(a), 0f) * radius;
                Vector3 p2 = center + new Vector3(Mathf.Cos(b), Mathf.Sin(b), 0f) * radius;

                Debug.DrawLine(p1, p2, color, 0f);
            }

            // XZ circle
            for (int i = 0; i < segments; i++)
            {
                float a = Mathf.Deg2Rad * (i * angleStep);
                float b = Mathf.Deg2Rad * ((i + 1) * angleStep);

                Vector3 p1 = center + new Vector3(Mathf.Cos(a), 0f, Mathf.Sin(a)) * radius;
                Vector3 p2 = center + new Vector3(Mathf.Cos(b), 0f, Mathf.Sin(b)) * radius;

                Debug.DrawLine(p1, p2, color, 0f);
            }

            // YZ circle
            for (int i = 0; i < segments; i++)
            {
                float a = Mathf.Deg2Rad * (i * angleStep);
                float b = Mathf.Deg2Rad * ((i + 1) * angleStep);

                Vector3 p1 = center + new Vector3(0f, Mathf.Cos(a), Mathf.Sin(a)) * radius;
                Vector3 p2 = center + new Vector3(0f, Mathf.Cos(b), Mathf.Sin(b)) * radius;

                Debug.DrawLine(p1, p2, color, 0f);
            }
        }
    }
}