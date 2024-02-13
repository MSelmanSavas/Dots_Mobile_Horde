using Unity.Entities;

[System.Serializable]
public struct ProjectileSpawnCooldownComponent : IComponentData
{
    public float CurrentCooldown;
    public float MaximumCooldown;
}
