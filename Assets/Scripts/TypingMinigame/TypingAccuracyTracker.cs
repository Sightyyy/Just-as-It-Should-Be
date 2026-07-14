public sealed class TypingAccuracyTracker
{
    private int correctLetters;
    private int requiredLetters;

    public float AccuracyPercent => requiredLetters == 0 ? 100f : (float)correctLetters / requiredLetters * 100f;
    public int CorrectLetters => correctLetters;
    public int RequiredLetters => requiredLetters;

    public void Reset()
    {
        correctLetters = 0;
        requiredLetters = 0;
    }

    public int RecordAttempt(string expectedWord, string playerInput)
    {
        int correct = CountCorrectLetters(expectedWord, playerInput);
        correctLetters += correct;
        requiredLetters += string.IsNullOrEmpty(expectedWord) ? 0 : expectedWord.Length;
        return correct;
    }

    private static int CountCorrectLetters(string expectedWord, string playerInput)
    {
        if (string.IsNullOrEmpty(expectedWord) || string.IsNullOrEmpty(playerInput))
        {
            return 0;
        }

        int count = 0;
        int length = expectedWord.Length < playerInput.Length ? expectedWord.Length : playerInput.Length;

        for (int i = 0; i < length; i++)
        {
            if (char.ToLowerInvariant(expectedWord[i]) == char.ToLowerInvariant(playerInput[i]))
            {
                count++;
            }
        }

        return count;
    }
}
