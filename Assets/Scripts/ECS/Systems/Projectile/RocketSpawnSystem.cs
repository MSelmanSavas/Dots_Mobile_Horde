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
        if (!SystemAPI.TryGetSingletonRW(out RefRW<RocketSpawnDataComponent> rocketSpawnDataComponent))
            return;

        if (!TryCheckCooldown(ref rocketSpawnDataComponent.ValueRW.SpawnCooldown, SystemAPI.Time.DeltaTime))
            return;

        if (!SystemAPI.TryGetSingletonEntity<PlayerTagComponent>(out Entity playerEntity))
            return;

        var playerLocalTransform = state.EntityManager.GetComponentData<LocalTransform>(playerEntity);

        EntityCommandBuffer entityCommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        float3 randomLinear = _random.NextFloat3Direction();
        randomLinear.z = 0;
        float3 normalized = math.normalizesafe(randomLinear);

        float angle = Vector3.SignedAngle(Vector3.right, normalized, Vector3.forward);

        var createdEntity = entityCommandBuffer.Instantiate(rocketSpawnDataComponent.ValueRO.Prefab);

        entityCommandBuffer.SetComponent(createdEntity, new LocalTransform
        {
            Position = playerLocalTransform.Position,
            Rotation = Quaternion.Euler(new float3(0f, 0f, angle)),
            Scale = 1f,
        });

        entityCommandBuffer.SetComponent(createdEntity, new PhysicsVelocity
        {
            Linear = normalized * 5f,
            Angular = 0f,
        });

    }

    [BurstCompile]
    bool TryCheckCooldown(ref ProjectileSpawnCooldownComponent cooldown, float deltaTime)
    {
        if (cooldown.CurrentCooldown > 0f)
        {
            cooldown.CurrentCooldown -= deltaTime;
            return false;
        }

        cooldown.CurrentCooldown = cooldown.MaximumCooldown;
        return true;
    }
}
