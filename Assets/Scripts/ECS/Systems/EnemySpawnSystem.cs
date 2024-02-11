using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Collections;


public partial class EnemySpawnSystem : SystemBase
{
    EntityCommandBuffer _entityCommandBuffer;
    Entity _enemyEntity;
    DynamicBuffer<EnemySpawnerDataComponent> _enemyDatas;


    protected override void OnUpdate()
    {
        _enemyDatas = SystemAPI.GetSingletonBuffer<EnemySpawnerDataComponent>();
        _entityCommandBuffer = World.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>().CreateCommandBuffer();

        for (int i = 0; i < _enemyDatas.Length; i++)
        {
            var data = _enemyDatas[i];

            if (data.CurrentAmountSpawned >= data.MaxAmountSpawned)
                continue;

            var createdEntity = _entityCommandBuffer.Instantiate(data.Prefab);

            float3 randomPosition = UnityEngine.Random.insideUnitSphere.normalized * 15;
            randomPosition.z = 0;

            _entityCommandBuffer.SetComponent(createdEntity, new LocalTransform
            {
                Position = randomPosition,
                Rotation = Quaternion.identity,
                Scale = 1f,
            });

            _entityCommandBuffer.SetComponent(createdEntity, new EnemyTagComponent
            {
                EnemySpawnerDataIndex = data.EntitySpawnerDataIndex,
            });

            data.CurrentAmountSpawned = data.CurrentAmountSpawned + 1;

            _enemyDatas[i] = data;
        }
    }
}
