using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class SkillUpgradeMenuController : MonoBehaviour
{
    [SerializeField] private SkillUpgradeManager upgradeManager;

    [Header("Nodes ordered from top (level 1) to bottom (max level)")]
    [SerializeField] private Button[] convincingDamageNodes;
    [SerializeField] private Button[] feelingsFortificationNodes;
    [SerializeField] private Button[] clearMindNodes;

    [Header("Optional visual feedback (leave empty to skip)")]
    [SerializeField] private Color lockedColor = new Color(0.5f, 0.5f, 0.5f);
    [SerializeField] private Color availableColor = Color.white;
    [SerializeField] private Color unlockedColor = new Color(0.4f, 0.9f, 0.4f);

    private readonly List<(SkillUpgradeType type, Button[] nodes)> nodeGroups = new();
    private bool listenersRegistered;

    void Awake()
    {
        upgradeManager ??= SkillUpgradeManager.Instance;

        nodeGroups.Clear();
        nodeGroups.Add((SkillUpgradeType.ConvincingDamage, convincingDamageNodes));
        nodeGroups.Add((SkillUpgradeType.FeelingsFortification, feelingsFortificationNodes));
        nodeGroups.Add((SkillUpgradeType.ClearMind, clearMindNodes));

        RegisterListeners();
    }

    void OnEnable()
    {
        upgradeManager ??= SkillUpgradeManager.Instance;
        Refresh();
    }

    private void RegisterListeners()
    {
        if (listenersRegistered) return;
        listenersRegistered = true;

        foreach (var group in nodeGroups)
        {
            var nodes = group.nodes;
            if (nodes == null) continue;

            for (int i = 0; i < nodes.Length; i++)
            {
                var button = nodes[i];
                if (button == null) continue;

                int nodeIndex = i; // capture for closure
                var type = group.type;
                button.onClick.AddListener(() => OnNodeClicked(type, nodeIndex));
            }
        }
    }

    private void OnNodeClicked(SkillUpgradeType type, int nodeIndex)
    {
        if (upgradeManager == null) return;

        if (nodeIndex != upgradeManager.GetLevel(type)) return;

        upgradeManager.Upgrade(type);
        Refresh();
    }

    public void Refresh()
    {
        if (upgradeManager == null)
        {
            upgradeManager = SkillUpgradeManager.Instance;
        }

        if (upgradeManager == null) return;

        foreach (var group in nodeGroups)
        {
            RefreshGroup(group.type, group.nodes);
        }
    }

    private void RefreshGroup(SkillUpgradeType type, Button[] nodes)
    {
        if (nodes == null) return;

        int currentLevel = upgradeManager.GetLevel(type);

        for (int i = 0; i < nodes.Length; i++)
        {
            var button = nodes[i];
            if (button == null) continue;

            bool isUnlocked = i < currentLevel;
            bool isAvailable = i == currentLevel;

            button.interactable = isAvailable;

            var graphic = button.targetGraphic;
            if (graphic != null)
            {
                graphic.color = isUnlocked ? unlockedColor : isAvailable ? availableColor : lockedColor;
            }
        }
    }
}