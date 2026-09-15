using FrameCoreU.Events;
using FrameVR.Player.Hands;
using HighlightPlus;
using Sirenix.OdinInspector;
using UnityEngine;

namespace FrameVR.Player.Interaction
{
    public class InteractableButton : MonoBehaviour, IHandInteractable
    {
        public enum ButtonMode
        {
            Press,
            Toggle
        }

        [BoxGroup("Settings")]
        [SerializeField] private bool active = true;
        
        [BoxGroup("Settings")]
        [SerializeField] private ButtonMode buttonMode = ButtonMode.Press;

        [BoxGroup("Settings")]
        [SerializeField] private HighlightEffect highlightEffect;

        [FoldoutGroup("Events")]
        
        [ShowIf("@buttonMode == ButtonMode.Press")]
        [FoldoutGroup("Events/Pressed")]
        [HideLabel]
        public FrameCoreEvent onPressed = new FrameCoreEvent { eventName = "Button Pressed" };

        [ShowIf("@buttonMode == ButtonMode.Press")]
        [FoldoutGroup("Events/Released")]
        [HideLabel]
        public FrameCoreEvent onReleased = new FrameCoreEvent { eventName = "Button Released" };

        [ShowIf("@buttonMode == ButtonMode.Toggle")]
        [FoldoutGroup("Events/Toggle On")]
        [HideLabel]
        public FrameCoreEvent onToggledOn = new FrameCoreEvent { eventName = "Button Toggled On" };

        [ShowIf("@buttonMode == ButtonMode.Toggle")]
        [FoldoutGroup("Events/Toggle Off")]
        [HideLabel]
        public FrameCoreEvent onToggledOff = new FrameCoreEvent { eventName = "Button Toggled Off" };

        [FoldoutGroup("Debug")]
        [SerializeField, ReadOnly] private bool toggled;

        public bool CanBeHeld => false;
        public bool CanBeInteractedWith => active;
        public HighlightEffect HighlightEffect => highlightEffect;

        public void OnHeld(PlayerHand hand) { }
        public void OnReleased(PlayerHand hand) { }

        public void OnGripStart(PlayerHand hand) { }
        public void OnGripEnd(PlayerHand hand) { }

        public void OnTriggerPressed(PlayerHand hand)
        {
            if (!active)
                return;

            onPressed.Activate();

            if (buttonMode != ButtonMode.Toggle)
                return;

            toggled = !toggled;

            if (toggled)
                onToggledOn.Activate();
            else
                onToggledOff.Activate();
        }

        public void OnTriggerReleased(PlayerHand hand)
        {
            if (!active)
                return;

            onReleased.Activate();
        }

        public void OnPrimaryPressed(PlayerHand hand) { }
        public void OnPrimaryReleased(PlayerHand hand) { }
        public void OnSecondaryPressed(PlayerHand hand) { }
        public void OnSecondaryReleased(PlayerHand hand) { }
    }
}