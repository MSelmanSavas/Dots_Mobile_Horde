using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;
using Unity.Burst;

[UpdateAfter(typeof(EnemySpawnSystem))]
public partial class EnemyDestroySystem : SystemBase
{
    EntityCommandBuffer _entityCommandBuffer;
    float _maxWaitTime;
    float _currentWaitTime;
    Unity.Mathematics.Random _random;


    protected override void OnCreate()
    {
        _random = new Unity.Mathematics.Random(10000);
        _maxWaitTime = 3f;
        _currentWaitTime = _maxWaitTime;
        //systemState.RequireForUpdate<EnemySpawnSystem>();
    }

    protected override void OnUpdate()
    {
        if (!TryCheckCooldown(SystemAPI.Time.DeltaTime))
            return;

        DynamicBuffer<EnemySpawnerDataComponent> enemySpawnDatas = SystemAPI.GetSingletonBuffer<EnemySpawnerDataComponent>();
        EntityCommandBuffer entityCommandBuffer = World.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>().CreateCommandBuffer();

        int randomAmount = _random.NextInt(0, 180);
        int currentAmount = 0;

        foreach (var (enemyTagComponent, enemyEntity) in SystemAPI.Query<EnemyTagComponent>().WithEntityAccess())
        {
            if (currentAmount >= randomAmount)
                return;

            int enemySpawnIndex = enemyTagComponent.EnemySpawnerDataIndex;

            var spawnData = enemySpawnDatas[enemySpawnIndex];
            spawnData.CurrentAmountSpawned -= 1;
            enemySpawnDatas[enemySpawnIndex] = spawnData;

            entityCommandBuffer.DestroyEntity(enemyEntity);
            currentAmount++;
        }
    }

    private bool TryCheckCooldown(float deltaTime)
    {
        if (_currentWaitTime > 0f)
        {
            _currentWaitTime -= deltaTime;
            return false;
        }

        _currentWaitTime = _maxWaitTime;
        return true;
    }
}
