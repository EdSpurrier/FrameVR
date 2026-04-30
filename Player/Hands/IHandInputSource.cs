namespace FrameVR.Player.Hands
{
    public interface IHandInputSource
    {
        float Grip { get; }
        float Trigger { get; }
        bool PrimaryPressed { get; }
        bool SecondaryPressed { get; }
    }
}