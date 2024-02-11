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
    float _currentWaitTime;
    float _maxWaitTime;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _random = new Unity.Mathematics.Random(15000);
        _maxWaitTime = 4f;
        _currentWaitTime = _maxWaitTime;
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (!TryCheckCooldown(SystemAPI.Time.DeltaTime))
            return;

        if (!SystemAPI.TryGetSingleton(out RocketSpawnDataComponent rocketSpawnDataComponent))
            return;

        if (!SystemAPI.TryGetSingletonEntity<PlayerTagComponent>(out Entity playerEntity))
            return;

        var playerLocalTransform = state.EntityManager.GetComponentData<LocalTransform>(playerEntity);

        EntityCommandBuffer entityCommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

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

    bool TryCheckCooldown(float deltaTime)
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
