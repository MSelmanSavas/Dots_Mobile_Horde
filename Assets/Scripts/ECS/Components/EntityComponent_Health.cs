using Unity.Entities;

public struct EntityComponent_Health : IComponentData
{
    public float CurrentHealth;
    public float MaxHealth;
    public bool IsDead;
}
