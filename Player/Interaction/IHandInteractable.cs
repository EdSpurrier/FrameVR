using FrameVR.Player.Hands;
using HighlightPlus;

namespace FrameVR.Player.Interaction
{
    public interface IHandInteractable
    {
        bool CanBeHeld { get; }

        HighlightEffect HighlightEffect { get; }

        void OnHoverStart(PlayerHand hand)
        {
            if (HighlightEffect && CanBeHeld) HighlightEffect.highlighted = true;
        }
        void OnHoverEnd(PlayerHand hand){
            if (HighlightEffect && CanBeHeld) HighlightEffect.highlighted = false;
        }
        
        void OnHeld(PlayerHand hand);
        void OnReleased(PlayerHand hand);

        void OnGripStart(PlayerHand hand);
        void OnGripEnd(PlayerHand hand);
        void OnTriggerPressed(PlayerHand hand);
        void OnTriggerReleased(PlayerHand hand);
        void OnPrimaryPressed(PlayerHand hand);
        void OnPrimaryReleased(PlayerHand hand);
        void OnSecondaryPressed(PlayerHand hand);
        void OnSecondaryReleased(PlayerHand hand);
    }
}