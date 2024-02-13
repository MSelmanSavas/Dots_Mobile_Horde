using Unity.Entities;

[System.Serializable]
public struct GenericCooldownComponent : IComponentData
{
    public float CurrentCooldown;
    public float MaxCooldown;
}
