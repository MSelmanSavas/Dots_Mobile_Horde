using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PhysicsSystemGroup))]
[BurstCompile]
public partial struct RocketTriggerSystem : ISystem
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
        systemState.Dependency = new RocketTriggerJob
        {
            EnemyGroup = SystemAPI.GetComponentLookup<EnemyTagComponent>(),
            EnemyHealthGroup = SystemAPI.GetComponentLookup<EntityComponent_Health>(),
            RocketGroup = SystemAPI.GetComponentLookup<RocketTagComponent>(),
            ECBParallel = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(systemState.WorldUnmanaged).AsParallelWriter(),
        }.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), systemState.Dependency);
    }


    [BurstCompile]
    struct RocketTriggerJob : ITriggerEventsJob
    {
        public ComponentLookup<EnemyTagComponent> EnemyGroup;
        public ComponentLookup<EntityComponent_Health> EnemyHealthGroup;
        public ComponentLookup<RocketTagComponent> RocketGroup;
        public EntityCommandBuffer.ParallelWriter ECBParallel;

        public void Execute(TriggerEvent triggerEvent)
        {
            Entity entityA = triggerEvent.EntityA;
            Entity entityB = triggerEvent.EntityB;

            bool isBodyARocket = RocketGroup.HasComponent(entityA);
            bool isBodyBRocket = RocketGroup.HasComponent(entityB);

            bool isBodyAEnemy = EnemyGroup.HasComponent(entityA);
            bool isBodyBEnemy = EnemyGroup.HasComponent(entityB);

            if (isBodyARocket && isBodyBRocket)
                return;

            if (isBodyAEnemy && isBodyBEnemy)
                return;

            if (!isBodyARocket && !isBodyBRocket)
                return;

            if (!isBodyAEnemy && !isBodyBEnemy)
                return;

            var RocketEntity = isBodyARocket ? entityA : entityB;
            var enemyEntity = isBodyAEnemy ? entityA : entityB;

            var enemyHealthComponent = EnemyHealthGroup[enemyEntity];

            enemyHealthComponent.ChangeHealth(-50);
            EnemyHealthGroup[enemyEntity] = enemyHealthComponent;

            ECBParallel.DestroyEntity(RocketEntity.Index, RocketEntity);
        }
    }
}
