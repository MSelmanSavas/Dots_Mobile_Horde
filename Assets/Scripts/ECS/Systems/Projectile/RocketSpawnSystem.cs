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
    float3 _lastCachedPlayerMovementDirection;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _random = new Unity.Mathematics.Random(15000);
        _lastCachedPlayerMovementDirection = float3.zero;
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.TryGetSingletonEntity<PlayerTagComponent>(out Entity playerEntity))
            return;

        var playerLocalTransform = state.EntityManager.GetComponentData<LocalTransform>(playerEntity);
        var playerMovementComponent = state.EntityManager.GetComponentData<PlayerMovementComponent>(playerEntity);

        if (playerMovementComponent.IsMoving)
            _lastCachedPlayerMovementDirection = playerMovementComponent.MovementVector;

        if (!SystemAPI.TryGetSingletonRW(out RefRW<RocketSpawnDataComponent> rocketSpawnDataComponent))
            return;

        if (!TryCheckCooldown(ref rocketSpawnDataComponent.ValueRW.SpawnCooldown, SystemAPI.Time.DeltaTime))
            return;

        int spawnCount = rocketSpawnDataComponent.ValueRO.SpawnCount;

        for (int i = 0; i < spawnCount; i++)
        {
            if (math.length(_lastCachedPlayerMovementDirection) == 0f)
            {
                float3 randomLinear = _random.NextFloat3Direction();
                randomLinear.z = 0;
                _lastCachedPlayerMovementDirection = randomLinear;
            }

            float3 normalized = math.normalizesafe(_lastCachedPlayerMovementDirection);

            float angle = Vector3.SignedAngle(Vector3.right, normalized, Vector3.forward);

            var createdEntity = state.EntityManager.Instantiate(rocketSpawnDataComponent.ValueRO.Prefab);

            state.EntityManager.SetComponentData(createdEntity, new LocalTransform
            {
                Position = playerLocalTransform.Position,
                Rotation = Quaternion.Euler(new float3(0f, 0f, angle)),
                Scale = 1f,
            });

            var startSpeedComponent = state.EntityManager.GetComponentData<ProjectileStartSpeedComponent>(createdEntity);

            state.EntityManager.SetComponentData(createdEntity, new PhysicsVelocity
            {
                Linear = normalized * startSpeedComponent.StartSpeed,
                Angular = 0f,
            });
        }
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
