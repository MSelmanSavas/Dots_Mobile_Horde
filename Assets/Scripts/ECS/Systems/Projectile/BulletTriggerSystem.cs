using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PhysicsSystemGroup))]
[BurstCompile]
public partial struct BulletTriggerSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EnemyTagComponent>();
        state.RequireForUpdate<SimulationSingleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState systemState)
    {
        systemState.Dependency = new BulletTriggerJob
        {
            EnemyGroup = SystemAPI.GetComponentLookup<EnemyTagComponent>(),
            EnemyHealthGroup = SystemAPI.GetComponentLookup<EntityComponent_Health>(),
            BulletGroup = SystemAPI.GetComponentLookup<BulletTagComponent>(),
            ECBParallel = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(systemState.WorldUnmanaged).AsParallelWriter(),
        }.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), systemState.Dependency);
    }


    [BurstCompile]
    struct BulletTriggerJob : ITriggerEventsJob
    {
        public ComponentLookup<EnemyTagComponent> EnemyGroup;
        public ComponentLookup<EntityComponent_Health> EnemyHealthGroup;
        public ComponentLookup<BulletTagComponent> BulletGroup;
        public EntityCommandBuffer.ParallelWriter ECBParallel;

        public void Execute(TriggerEvent triggerEvent)
        {
            Entity entityA = triggerEvent.EntityA;
            Entity entityB = triggerEvent.EntityB;

            bool isBodyABullet = BulletGroup.HasComponent(entityA);
            bool isBodyBBullet = BulletGroup.HasComponent(entityB);

            bool isBodyAEnemy = EnemyGroup.HasComponent(entityA);
            bool isBodyBEnemy = EnemyGroup.HasComponent(entityB);

            if (isBodyABullet && isBodyBBullet)
                return;

            if (isBodyAEnemy && isBodyBEnemy)
                return;

            if (!isBodyABullet && !isBodyBBullet)
                return;

            if (!isBodyAEnemy && !isBodyBEnemy)
                return;

            var bulletEntity = isBodyABullet ? entityA : entityB;
            var enemyEntity = isBodyAEnemy ? entityA : entityB;

            var enemyHealthComponent = EnemyHealthGroup[enemyEntity];

            enemyHealthComponent.ChangeHealth(-50);
            EnemyHealthGroup[enemyEntity] = enemyHealthComponent;

            ECBParallel.DestroyEntity(bulletEntity.Index, bulletEntity);
        }
    }
}
