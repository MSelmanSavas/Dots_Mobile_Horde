using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

[BurstCompile]
public partial struct ProjectileDestroyAfterLifetimeSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState systemState)
    {
        var checkProjectileLifetimeQuery = SystemAPI.QueryBuilder()
             .WithAllRW<ProjectileSelfDestructComponent>().Build();

        var checkProjectileLifetimeHandle = new CheckProjectileLifetime()
        {
            DeltaTime = SystemAPI.Time.DeltaTime,
            ECBParallel = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(systemState.WorldUnmanaged).AsParallelWriter(),
        };
        systemState.Dependency = checkProjectileLifetimeHandle.ScheduleParallel(checkProjectileLifetimeQuery, systemState.Dependency);
    }

    [BurstCompile]
    partial struct CheckProjectileLifetime : IJobEntity
    {
        public float DeltaTime;
        public EntityCommandBuffer.ParallelWriter ECBParallel;
        public void Execute(Entity projectile, ref ProjectileSelfDestructComponent projectileSelfDestructComponent)
        {
            if (projectileSelfDestructComponent.PassedTime <= projectileSelfDestructComponent.TimeToLive)
            {
                projectileSelfDestructComponent.PassedTime += DeltaTime;
                return;
            }

            ECBParallel.DestroyEntity(projectile.Index, projectile);
        }
    }
}
