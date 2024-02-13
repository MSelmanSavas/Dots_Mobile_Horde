using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using UnityEngine;

public partial struct EnemyAttackPlayerOnCollisionSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EnemyTagComponent>();
        state.RequireForUpdate<PlayerTagComponent>();
        state.RequireForUpdate<SimulationSingleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState systemState)
    {
        systemState.Dependency = new EnemyAttackOnCollisionJob
        {
            EnemyGroup = SystemAPI.GetComponentLookup<EnemyTagComponent>(),
            PlayerHealthGroup = SystemAPI.GetComponentLookup<EntityComponent_Health>(),
            PlayerGroup = SystemAPI.GetComponentLookup<PlayerTagComponent>(),
            EnemyAttackGroup = SystemAPI.GetComponentLookup<EnemyAttackComponent>(),
            TimeDelta = SystemAPI.Time.DeltaTime,
        }.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), systemState.Dependency);
    }

    [BurstCompile]
    struct EnemyAttackOnCollisionJob : ICollisionEventsJob
    {
        public ComponentLookup<EnemyTagComponent> EnemyGroup;
        public ComponentLookup<EntityComponent_Health> PlayerHealthGroup;
        public ComponentLookup<PlayerTagComponent> PlayerGroup;
        public ComponentLookup<EnemyAttackComponent> EnemyAttackGroup;
        public float TimeDelta;

        public void Execute(CollisionEvent collisionEvent)
        {
            Entity entityA = collisionEvent.EntityA;
            Entity entityB = collisionEvent.EntityB;

            bool isBodyAPlayer = PlayerGroup.HasComponent(entityA);
            bool isBodyBPlayer = PlayerGroup.HasComponent(entityB);

            bool isBodyAEnemy = EnemyGroup.HasComponent(entityA);
            bool isBodyBEnemy = EnemyGroup.HasComponent(entityB);

            if (isBodyAPlayer && isBodyBPlayer)
                return;

            if (isBodyAEnemy && isBodyBEnemy)
                return;

            if (!isBodyAPlayer && !isBodyBPlayer)
                return;

            if (!isBodyAEnemy && !isBodyBEnemy)
                return;

            var playerEntity = isBodyAPlayer ? entityA : entityB;
            var enemyEntity = isBodyAEnemy ? entityA : entityB;

            var enemyAttackComponent = EnemyAttackGroup[enemyEntity];

            if (enemyAttackComponent.AttackCooldown.CurrentCooldown > 0f)
            {
                enemyAttackComponent.AttackCooldown.CurrentCooldown -= TimeDelta;
                EnemyAttackGroup[enemyEntity] = enemyAttackComponent;
                return;
            }

            enemyAttackComponent.AttackCooldown.CurrentCooldown = enemyAttackComponent.AttackCooldown.MaxCooldown;
            EnemyAttackGroup[enemyEntity] = enemyAttackComponent;

            var playerHealthComponent = PlayerHealthGroup[playerEntity];

            playerHealthComponent.ChangeHealth(-enemyAttackComponent.DamageAmount);
            PlayerHealthGroup[playerEntity] = playerHealthComponent;
        }
    }
}
