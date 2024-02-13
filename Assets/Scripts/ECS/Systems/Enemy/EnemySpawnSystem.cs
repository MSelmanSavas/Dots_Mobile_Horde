using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Rendering;
using System.Collections.Generic;
using UnityEngine.Rendering;
using Unity.Collections;
using System.Linq;
using Unity.Burst;


[CreateAfter(typeof(PlayerMovementSyncEntitySystem))]
[UpdateAfter(typeof(PlayerMovementSyncEntitySystem))]
[RequireMatchingQueriesForUpdate]
public partial struct EnemySpawnSystem : ISystem
{
    EntityCommandBuffer _entityCommandBuffer;
    DynamicBuffer<EnemySpawnerDataComponent> _enemyDatas;

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.TryGetSingletonBuffer(out _enemyDatas))
            return;

        if (!SystemAPI.TryGetSingletonEntity<PlayerTagComponent>(out Entity playerEntity))
            return;

        PlayerAspect playerAspect = state.EntityManager.GetAspect<PlayerAspect>(playerEntity);

        _entityCommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        for (int i = 0; i < _enemyDatas.Length; i++)
        {
            var data = _enemyDatas[i];

            if (data.CurrentAmountSpawned >= data.MaxAmountSpawned)
                continue;

            if (!TryCheckCooldown(data, SystemAPI.Time.DeltaTime))
            {
                _enemyDatas[i] = data;
                continue;
            }

            var createdEntity = _entityCommandBuffer.Instantiate(data.Prefab);

            float3 randomPosition = UnityEngine.Random.insideUnitSphere;
            randomPosition.z = 0;
            randomPosition = math.normalizesafe(randomPosition) * 30;

            randomPosition += playerAspect.PlayerTransform.ValueRO.Position;

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

            data.CurrentAmountSpawned += 1;
            _enemyDatas[i] = data;
        }
    }

    bool TryCheckCooldown(EnemySpawnerDataComponent enemySpawnerData, float deltaTime)
    {
        ref var genericCooldown = ref enemySpawnerData.GenericCooldown;

        if (genericCooldown.CurrentCooldown > 0f)
        {
            genericCooldown.CurrentCooldown -= deltaTime;
            return false;
        }

        genericCooldown.CurrentCooldown = genericCooldown.MaxCooldown;
        return true;
    }
}
