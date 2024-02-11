using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;
using Unity.Burst;

[UpdateAfter(typeof(EnemySpawnSystem))]
public partial class EnemyDestroySystem : SystemBase
{
    protected override void OnUpdate()
    {
        DynamicBuffer<EnemySpawnerDataComponent> enemySpawnDatas = SystemAPI.GetSingletonBuffer<EnemySpawnerDataComponent>();

        EntityCommandBuffer entityCommandBuffer = World.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>().CreateCommandBuffer();

        foreach (var (enemyTagComponent, enemyHealthComponent, enemyEntity) in SystemAPI.Query<EnemyTagComponent, EntityComponent_Health>().WithEntityAccess())
        {
            if (!enemyHealthComponent.IsDead)
                continue;

            int enemySpawnIndex = enemyTagComponent.EnemySpawnerDataIndex;

            var spawnData = enemySpawnDatas[enemySpawnIndex];
            spawnData.CurrentAmountSpawned -= 1;
            enemySpawnDatas[enemySpawnIndex] = spawnData;

            entityCommandBuffer.DestroyEntity(enemyEntity);
        }
    }
}
