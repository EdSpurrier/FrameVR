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
        
        [Title("Hold")]
        public HoldAttachMode holdAttachMode = HoldAttachMode.SnapInstant;
        public float pullToHandSpeed = 12f;
        public float pullSnapDistance = 0.03f;
    }
}