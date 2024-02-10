using Unity.Entities;
using Unity.Entities.Serialization;

[System.Serializable]
public struct EnemySpawnerDataComponent : IBufferElementData
{
    public Entity Prefab;
    public int CurrentAmountSpawned;
    public int MaxAmountSpawned;
}
