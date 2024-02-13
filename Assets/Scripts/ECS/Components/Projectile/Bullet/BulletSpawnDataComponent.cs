using Unity.Entities;

public struct BulletSpawnDataComponent : IComponentData
{
    public Entity Prefab;
    public ProjectileSpawnCooldownComponent SpawnCooldown;
    public ProjectileStartSpeedComponent StartSpeed;
}
