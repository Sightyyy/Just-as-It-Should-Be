using UnityEngine;

public sealed class TypingMonster
{
    private readonly int maxHealth;
    private readonly float regenPercentPerSecond;

    private float currentHealth;

    public TypingMonster(PlayerEmotionalState state, int maxHealth, float regenPercentPerSecond)
    {
        State = state;
        this.maxHealth = Mathf.Max(1, maxHealth);
        this.regenPercentPerSecond = Mathf.Max(0f, regenPercentPerSecond);
        currentHealth = this.maxHealth;
    }

    public PlayerEmotionalState State { get; }
    public int MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;
    public bool IsDead => currentHealth <= 0f;
    public float HealthNormalized => Mathf.Clamp01(currentHealth / maxHealth);

    public void Damage(int amount)
    {
        if (amount <= 0 || IsDead) return;

        currentHealth = Mathf.Max(0f, currentHealth - amount);
    }

    public void Regenerate(float deltaTime)
    {
        if (IsDead || currentHealth >= maxHealth) return;

        float amount = maxHealth * regenPercentPerSecond * deltaTime;
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
    }
}
