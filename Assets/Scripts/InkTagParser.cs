using System.Collections.Generic;
using UnityEngine;

public sealed class DialogueTagParser
{
    private const string SpeakerTag = "speaker";
    private const string PortraitTag = "portrait";

    public void Apply(IEnumerable<string> tags, DialogueState state)
    {
        if (tags == null || state == null)
        {
            return;
        }

        foreach (string tag in tags)
        {
            if (string.IsNullOrWhiteSpace(tag))
            {
                continue;
            }

            int separatorIndex = tag.IndexOf(':');
            if (separatorIndex <= 0 || separatorIndex == tag.Length - 1)
            {
                Debug.LogWarning($"Ignoring malformed Ink tag '{tag}'. Use 'speaker: Name' or 'portrait: animator_state'.");
                continue;
            }

            string key = tag.Substring(0, separatorIndex).Trim().ToLowerInvariant();
            string value = tag.Substring(separatorIndex + 1).Trim();

            switch (key)
            {
                case SpeakerTag:
                    state.SetSpeaker(value);
                    break;
                case PortraitTag:
                    state.SetPortraitState(value);
                    break;
                default:
                    Debug.LogWarning($"Ink dialogue tag '{key}' is not handled.");
                    break;
            }
        }
    }
}
