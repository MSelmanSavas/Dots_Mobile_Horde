using Unity.Entities;
using Unity.Entities.Serialization;

[System.Serializable]
public struct EnemySpawnerData : IBufferElementData
{
    public Entity Prefab;
    public int CurrentAmountSpawned;
    public int MaxAmountSpawned;
}
