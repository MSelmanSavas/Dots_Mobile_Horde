using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

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
        if (!SystemAPI.TryGetSingleton(out LavaSpawnDataComponent lavaSpawnDataComponent))
            return;

        systemState.Dependency = new RocketTriggerJob
        {
            EnemyGroup = SystemAPI.GetComponentLookup<EnemyTagComponent>(),
            RocketGroup = SystemAPI.GetComponentLookup<RocketTagComponent>(),
            RocketTransformGroup = SystemAPI.GetComponentLookup<LocalTransform>(),
            ECBParallel = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(systemState.WorldUnmanaged).AsParallelWriter(),
            LavaSpawnData = lavaSpawnDataComponent,
        }.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), systemState.Dependency);
    }


    [BurstCompile]
    struct RocketTriggerJob : ITriggerEventsJob
    {
        public ComponentLookup<EnemyTagComponent> EnemyGroup;
        public ComponentLookup<RocketTagComponent> RocketGroup;
        public ComponentLookup<LocalTransform> RocketTransformGroup;
        public EntityCommandBuffer.ParallelWriter ECBParallel;
        public LavaSpawnDataComponent LavaSpawnData;

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

            var rocketEntity = isBodyARocket ? entityA : entityB;

            var rocketTransform = RocketTransformGroup[rocketEntity];

            var lavaEntity = ECBParallel.Instantiate(rocketEntity.Index, LavaSpawnData.Prefab);

            ECBParallel.SetComponent(rocketEntity.Index, lavaEntity, new LocalTransform
            {
                Position = rocketTransform.Position,
                Rotation = Unity.Mathematics.quaternion.identity,
                Scale = 1f,
            });

            ECBParallel.DestroyEntity(rocketEntity.Index, rocketEntity);
        }
    }
}
