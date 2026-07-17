using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class OxfordTypingDictionary : MonoBehaviour
{
    [SerializeField] private TextAsset oxfordWordList;
    [SerializeField] private int minimumWordLength = 3;
    [SerializeField] private int maximumWordLength = 12;

    private readonly List<string> words = new List<string>();
    private string lastWord;

    private static readonly string[] FallbackWords =
    {
        "texture",
        "brave",
        "memory",
        "gentle",
        "shadow",
        "honest",
        "future",
        "silent"
    };

    void Awake()
    {
        Reload();
    }

    public void Reload()
    {
        words.Clear();

        if (oxfordWordList != null)
        {
            string[] entries = oxfordWordList.text.Split(new[] { '\r', '\n', ',', ';', '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string entry in entries)
            {
                TryAddWord(entry);
            }
        }

        if (words.Count == 0)
        {
            foreach (string fallbackWord in FallbackWords)
            {
                TryAddWord(fallbackWord);
            }
        }
    }

    public string GetRandomWord()
    {
        if (words.Count == 0)
        {
            Reload();
        }

        string word = words[UnityEngine.Random.Range(0, words.Count)];

        if (words.Count > 1)
        {
            int attempts = 0;
            while (word == lastWord && attempts < 10)
            {
                word = words[UnityEngine.Random.Range(0, words.Count)];
                attempts++;
            }
        }

        lastWord = word;
        return word;
    }

    private void TryAddWord(string rawWord)
    {
        string word = rawWord.Trim().ToLowerInvariant();
        if (word.Length < minimumWordLength || word.Length > maximumWordLength) return;
        if (!IsAlphabetic(word)) return;
        if (words.Contains(word)) return;

        words.Add(word);
    }

    private static bool IsAlphabetic(string word)
    {
        for (int i = 0; i < word.Length; i++)
        {
            if (!char.IsLetter(word[i]))
            {
                return false;
            }
        }

        return true;
    }
}