using Unity.Entities;
using Unity.Mathematics;

[System.Serializable]
public struct EntityComponent_Health : IComponentData
{
    public float CurrentHealth;
    public float MaxHealth;
    public bool IsDead;

    public EntityComponent_Health GetFullHealth()
    {
        return new EntityComponent_Health
        {
            CurrentHealth = this.MaxHealth,
            MaxHealth = this.MaxHealth,
            IsDead = this.IsDead,
        };
    }

    public void ChangeHealth(float changeAmount)
    {
        CurrentHealth = math.clamp(CurrentHealth + changeAmount, 0f, MaxHealth);

        if (CurrentHealth > 0f)
            return;

        IsDead = true;
    }
}
