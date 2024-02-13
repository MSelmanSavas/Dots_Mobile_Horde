using Unity.Entities;
using Unity.Entities.Serialization;

public struct BulletSpawnDataComponent : IComponentData
{
    public Entity Prefab;
    public EntityPrefabReference PrefabReference;
}
