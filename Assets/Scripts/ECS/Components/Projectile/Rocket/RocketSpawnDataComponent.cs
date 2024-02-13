using Unity.Entities;

public struct RocketSpawnDataComponent : IComponentData
{
    public Entity Prefab;
    public ProjectileSpawnCooldownComponent SpawnCooldown;
}
