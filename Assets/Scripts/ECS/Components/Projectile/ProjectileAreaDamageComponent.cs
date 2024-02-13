using Unity.Entities;

[System.Serializable]
public struct ProjectileAreaDamageComponent : IComponentData
{
    public float CurrentCooldown;
    public float MaxCooldown;
    public ProjectileDirectDamageComponent DamageData;
}
