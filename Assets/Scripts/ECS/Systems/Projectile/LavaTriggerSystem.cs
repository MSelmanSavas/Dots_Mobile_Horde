using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PhysicsSystemGroup))]
[BurstCompile]
public partial struct LavaTriggerSystem : ISystem
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
        systemState.Dependency = new LavaTriggerJob
        {
            EnemyGroup = SystemAPI.GetComponentLookup<EnemyTagComponent>(),
            EnemyHealthGroup = SystemAPI.GetComponentLookup<EntityComponent_Health>(),
            LavaGroup = SystemAPI.GetComponentLookup<LavaTagComponent>(),
            LavaAreaDamageGroup = SystemAPI.GetComponentLookup<ProjectileAreaDamageComponent>(),
            ECBParallel = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(systemState.WorldUnmanaged).AsParallelWriter(),
            TimeDelta = SystemAPI.Time.DeltaTime,
        }.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), systemState.Dependency);
    }

    [BurstCompile]
    struct LavaTriggerJob : ITriggerEventsJob
    {
        public ComponentLookup<EnemyTagComponent> EnemyGroup;
        public ComponentLookup<EntityComponent_Health> EnemyHealthGroup;
        public ComponentLookup<LavaTagComponent> LavaGroup;
        public ComponentLookup<ProjectileAreaDamageComponent> LavaAreaDamageGroup;
        public EntityCommandBuffer.ParallelWriter ECBParallel;
        public float TimeDelta;

        public void Execute(TriggerEvent triggerEvent)
        {
            Entity entityA = triggerEvent.EntityA;
            Entity entityB = triggerEvent.EntityB;

            bool isBodyALava = LavaGroup.HasComponent(entityA);
            bool isBodyBLava = LavaGroup.HasComponent(entityB);

            bool isBodyAEnemy = EnemyGroup.HasComponent(entityA);
            bool isBodyBEnemy = EnemyGroup.HasComponent(entityB);

            if (isBodyALava && isBodyBLava)
                return;

            if (isBodyAEnemy && isBodyBEnemy)
                return;

            if (!isBodyALava && !isBodyBLava)
                return;

            if (!isBodyAEnemy && !isBodyBEnemy)
                return;

            var lavaEntity = isBodyALava ? entityA : entityB;
            var enemyEntity = isBodyAEnemy ? entityA : entityB;

            var lavaAreaDamageComponent = LavaAreaDamageGroup[lavaEntity];

            if (lavaAreaDamageComponent.CurrentCooldown >= 0f)
            {
                lavaAreaDamageComponent.CurrentCooldown -= TimeDelta;
                LavaAreaDamageGroup[lavaEntity] = lavaAreaDamageComponent;
                return;
            }

            lavaAreaDamageComponent.CurrentCooldown = lavaAreaDamageComponent.MaxCooldown;
            LavaAreaDamageGroup[lavaEntity] = lavaAreaDamageComponent;

            var enemyHealthComponent = EnemyHealthGroup[enemyEntity];

            enemyHealthComponent.ChangeHealth(-lavaAreaDamageComponent.DamageData.DamageAmount);
            EnemyHealthGroup[enemyEntity] = enemyHealthComponent;
        }
    }
}
