public sealed class DialogueState
{
    public string Speaker { get; private set; } = "???";
    public string PortraitState { get; private set; } = "npc_portrait";

    public void SetSpeaker(string speaker)
    {
        Speaker = string.IsNullOrWhiteSpace(speaker) ? "???" : speaker;
    }

    public void SetPortraitState(string portraitState)
    {
        PortraitState = portraitState;
    }

    public void Reset()
    {
        Speaker = "???";
        PortraitState = "npc_portrait";
    }
}
