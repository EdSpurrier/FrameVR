using Sirenix.OdinInspector;
using UnityEngine;

namespace FrameVR.Player.Hands
{
    [CreateAssetMenu(menuName = "Game/Player/Player Hand Settings")]
    public class PlayerHandSettings : ScriptableObject
    {
        public enum GripMode
        {
            Hold,
            Toggle
        }

        public enum HoldAttachMode
        {
            SnapInstant,
            PullToHand
        }
        
        public enum PickupMode
        {
            Near,
            Far,
            NearAndFar
        }
        
        [Title("Grip")]
        public GripMode gripMode = GripMode.Hold;

        [Title("Pickup")]
        public PickupMode pickupMode = PickupMode.NearAndFar;
        public LayerMask pickupMask = ~0;
        public float rayDistance = 4f;
        public float overlapRadius = 0.25f;
        public bool releaseOnGripEnd = true;
        
        
        [Title("Hold")]
        public HoldAttachMode holdAttachMode = HoldAttachMode.SnapInstant;
        public float pullToHandSpeed = 12f;
        public float pullSnapDistance = 0.03f;
        
        [Title("Throw")]
        public bool enableThrow = true;
        public int velocitySampleFrames = 6;
        public float throwMultiplier = 1.2f;
        public float maxThrowSpeed = 10f;
        public bool applyAngularVelocity = true;
    }
}