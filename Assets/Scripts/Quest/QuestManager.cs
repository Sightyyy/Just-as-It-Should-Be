using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class QuestManager : MonoBehaviour
{
    [Header("Quests")]
    [SerializeField] private List<QuestData> questCatalog;

    [Header("UI")]
    [Tooltip("RectTransform of the Quest Panel content that will hold the quest entries.")]
    [SerializeField] private RectTransform questListContent;
    [SerializeField] private TMP_FontAsset fontOverride;

    [Header("Style")]
    [SerializeField] private Color titleColor = new Color(0.95f, 0.9f, 0.7f);
    [SerializeField] private Color completedColor = new Color(0.5f, 0.5f, 0.5f);
    [SerializeField] private Color objectivePendingColor = new Color(0.85f, 0.85f, 0.85f);
    [SerializeField] private Color objectiveDoneColor = new Color(0.45f, 0.7f, 0.45f);

    private RectTransform entriesRoot;

    public static QuestManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("More than one QuestManager was found. The duplicate was disabled.", this);
            enabled = false;
            return;
        }

        Instance = this;
        BuildEntriesRoot();
    }

    void OnEnable()
    {
        SaveManager.AfterLoad += Render;

        if (DialogueProgress.Instance != null)
        {
            DialogueProgress.Instance.FlagChanged += OnFlagChanged;
        }
    }

    void OnDisable()
    {
        SaveManager.AfterLoad -= Render;

        if (DialogueProgress.Instance != null)
        {
            DialogueProgress.Instance.FlagChanged -= OnFlagChanged;
        }
    }

    void Start()
    {
        Render();
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public bool IsAccepted(QuestData quest)
    {
        return quest != null && DialogueProgress.Instance != null && DialogueProgress.Instance.HasFlag(quest.acceptFlag);
    }

    public bool IsCompleted(QuestData quest)
    {
        return quest != null && !string.IsNullOrWhiteSpace(quest.completeFlag)
            && DialogueProgress.Instance != null
            && DialogueProgress.Instance.HasFlag(quest.completeFlag);
    }

    public bool IsObjectiveDone(QuestData quest, QuestData.QuestObjective objective)
    {
        if (DialogueProgress.Instance == null)
        {
            return false;
        }

        string doneFlag = string.IsNullOrWhiteSpace(objective.doneFlag) ? quest.completeFlag : objective.doneFlag;
        return !string.IsNullOrWhiteSpace(doneFlag) && DialogueProgress.Instance.HasFlag(doneFlag);
    }

    public void Render(SaveData data)
    {
        Render();
    }

    public void Render()
    {
        if (entriesRoot == null)
        {
            return;
        }

        for (int i = entriesRoot.childCount - 1; i >= 0; i--)
        {
            Destroy(entriesRoot.GetChild(i).gameObject);
        }

        if (questCatalog == null)
        {
            return;
        }

        foreach (QuestData quest in questCatalog)
        {
            if (quest == null || !IsAccepted(quest))
            {
                continue;
            }

            AddQuestEntry(quest);
        }
    }

    private void OnFlagChanged(string flag)
    {
        Render();
    }

    private void BuildEntriesRoot()
    {
        if (questListContent == null)
        {
            Debug.LogError("QuestManager needs a Quest List Content reference.", this);
            return;
        }

        if (questListContent.GetComponent<VerticalLayoutGroup>() != null)
        {
            entriesRoot = questListContent;

            ContentSizeFitter fitter = questListContent.GetComponent<ContentSizeFitter>();
            if (fitter == null)
            {
                fitter = questListContent.gameObject.AddComponent<ContentSizeFitter>();
            }
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            return;
        }

        GameObject root = new GameObject("Quest Entries", typeof(RectTransform), typeof(VerticalLayoutGroup));
        entriesRoot = root.GetComponent<RectTransform>();
        entriesRoot.SetParent(questListContent, false);
        entriesRoot.anchorMin = Vector2.zero;
        entriesRoot.anchorMax = Vector2.one;
        entriesRoot.offsetMin = new Vector2(24f, 24f);
        entriesRoot.offsetMax = new Vector2(-24f, -24f);

        VerticalLayoutGroup layout = root.GetComponent<VerticalLayoutGroup>();
        layout.spacing = 10f;
        layout.padding = new RectOffset(0, 0, 8, 8);
        layout.childAlignment = TextAnchor.UpperLeft;
        layout.childControlHeight = true;
        layout.childControlWidth = true;
        layout.childForceExpandHeight = false;
        layout.childForceExpandWidth = true;
    }

    private void AddQuestEntry(QuestData quest)
    {
        bool completed = IsCompleted(quest);

        TextMeshProUGUI title = CreateText(
            completed ? $"{quest.title}  —  Completed" : quest.title,
            24,
            completed ? completedColor : titleColor,
            FontStyles.Bold
        );
        title.name = $"Quest Title - {quest.id}";

        TextMeshProUGUI description = CreateText(quest.description, 17, completed ? completedColor : objectivePendingColor, FontStyles.Italic);
        description.name = $"Quest Description - {quest.id}";

        if (quest.objectives != null)
        {
            foreach (QuestData.QuestObjective objective in quest.objectives)
            {
                bool done = completed || IsObjectiveDone(quest, objective);

                TextMeshProUGUI line = CreateText(
                    done ? $"✓  {objective.text}" : $"•  {objective.text}",
                    18,
                    done ? objectiveDoneColor : objectivePendingColor,
                    done ? FontStyles.Strikethrough : FontStyles.Normal
                );
                line.name = $"Quest Objective - {quest.id}";
            }
        }

        LayoutSpacer(14f);
    }

    private void LayoutSpacer(float height)
    {
        GameObject spacer = new GameObject("Quest Spacer", typeof(RectTransform), typeof(LayoutElement));
        spacer.transform.SetParent(entriesRoot, false);
        spacer.GetComponent<LayoutElement>().preferredHeight = height;
    }

    private TextMeshProUGUI CreateText(string content, int size, Color color, FontStyles style)
    {
        GameObject textObject = new GameObject("Quest Text", typeof(RectTransform), typeof(TextMeshProUGUI), typeof(LayoutElement));
        textObject.transform.SetParent(entriesRoot, false);

        TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();
        text.text = content;
        text.fontSize = size;
        text.color = color;
        text.fontStyle = style;
        text.enableWordWrapping = true;
        if (fontOverride != null)
        {
            text.font = fontOverride;
        }

        LayoutElement layout = textObject.GetComponent<LayoutElement>();
        layout.preferredHeight = -1f;

        return text;
    }
}
