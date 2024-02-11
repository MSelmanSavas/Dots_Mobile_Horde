using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;
using Unity.Burst;

[BurstCompile]
public partial struct RocketSpawnSystem : ISystem
{
    Unity.Mathematics.Random _random;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _random = new Unity.Mathematics.Random(15000);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.TryGetSingleton(out RocketSpawnDataComponent rocketSpawnDataComponent))
            return;

        if (!SystemAPI.TryGetSingletonEntity<PlayerTagComponent>(out Entity playerEntity))
            return;

        var playerLocalTransform = state.EntityManager.GetComponentData<LocalTransform>(playerEntity);

        EntityCommandBuffer entityCommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        for (int i = 0; i < 1; i++)
        {
            float3 randomLinear = _random.NextFloat3Direction();
            randomLinear.z = 0;
            float3 normalized = math.normalizesafe(randomLinear);

            var createdEntity = entityCommandBuffer.Instantiate(rocketSpawnDataComponent.Prefab);

            entityCommandBuffer.SetComponent(createdEntity, new LocalTransform
            {
                Position = playerLocalTransform.Position,
                Rotation = Quaternion.identity,
                Scale = 1f,
            });

            entityCommandBuffer.SetComponent(createdEntity, new PhysicsVelocity
            {
                Linear = normalized * 2f,
                Angular = 0f,
            });
        }
    }
}
