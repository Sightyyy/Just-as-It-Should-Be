using UnityEngine;

public static class InteractionInput
{
    public const KeyCode InteractionKey = KeyCode.Z;

    private static bool consumedForCurrentPress;
    private static int lastPressFrame = -1;

    public static bool TryConsumeInteraction()
    {
        RefreshPressState();

        if (!Input.GetKeyDown(InteractionKey) || consumedForCurrentPress)
        {
            return false;
        }

        consumedForCurrentPress = true;
        return true;
    }

    public static bool TryConsumeInteractionHold()
    {
        RefreshPressState();

        if (!Input.GetKey(InteractionKey) || consumedForCurrentPress)
        {
            return false;
        }

        consumedForCurrentPress = true;
        return true;
    }

    // The press stays latched until release so a Z that closes one interaction
    // cannot immediately start another (for example, reopening the same dialogue).
    private static void RefreshPressState()
    {
        if (!Input.GetKey(InteractionKey))
        {
            consumedForCurrentPress = false;
            return;
        }

        if (Input.GetKeyDown(InteractionKey) && Time.frameCount != lastPressFrame)
        {
            lastPressFrame = Time.frameCount;
            consumedForCurrentPress = false;
        }
    }
}
