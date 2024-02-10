using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;
using Unity.Burst;

[BurstCompile]
public partial struct EnemyDestroySystem : ISystem
{
    EntityCommandBuffer _entityCommandBuffer;

    float _maxWaitTime;
    float _currentWaitTime;
    Unity.Mathematics.Random _random;

    [BurstCompile]
    public void OnCreate(ref SystemState systemState)
    {
        _random = new Unity.Mathematics.Random(10000);
        _maxWaitTime = 1f;
        _currentWaitTime = _maxWaitTime;
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState systemState)
    {
        return;
        
        if (!TryCheckCooldown(SystemAPI.Time.DeltaTime))
            return;

        EntityCommandBuffer.ParallelWriter entityCommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(systemState.WorldUnmanaged).AsParallelWriter();

        //int randomAmount = _random.NextInt(0, 100);
        int randomAmount = 20;
        int currentAmount = 0;

        foreach (var (enemyTagComponent, enemyEntity) in SystemAPI.Query<EnemyTagComponent>().WithEntityAccess())
        {
            if (currentAmount >= randomAmount)
                return;

            entityCommandBuffer.DestroyEntity(enemyEntity.Index, enemyEntity);
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
