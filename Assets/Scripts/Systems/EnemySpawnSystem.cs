using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;


public partial class EnemySpawnSystem : SystemBase
{
    EntityCommandBuffer _entityCommandBuffer;
    Entity _enemyEntity;
    DynamicBuffer<EnemySpawnerData> _enemyDatas;

    protected override void OnStartRunning()
    {
        base.OnStartRunning();
        //_entityCommandBuffer = World.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>().CreateCommandBuffer();
    }

    protected override void OnUpdate()
    {
        _enemyDatas = SystemAPI.GetSingletonBuffer<EnemySpawnerData>();
        _entityCommandBuffer = World.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>().CreateCommandBuffer();

        for (int i = 0; i < _enemyDatas.Length; i++)
        {
            var data = _enemyDatas[i];

            if (data.CurrentAmountSpawned >= data.MaxAmountSpawned)
                continue;

            var createdEntity = _entityCommandBuffer.Instantiate(data.Prefab);

            float3 randomPosition = UnityEngine.Random.insideUnitSphere * 15;
            randomPosition.z = 0;


            _entityCommandBuffer.SetComponent<LocalTransform>(createdEntity, new LocalTransform
            {
                Position = randomPosition,
                Rotation = Quaternion.identity,
                Scale = 1f,
            });

            data.CurrentAmountSpawned = data.CurrentAmountSpawned + 1;

            _enemyDatas[i] = data;
        }

    }
}
