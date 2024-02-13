using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Rendering;

[BurstCompile]
public partial struct BulletSpawnSystem : ISystem
{
    Unity.Mathematics.Random _random;
    float _currentWaitTime;
    float _maxWaitTime;
    float3 _lastCachedPlayerMovementDirection;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _random = new Unity.Mathematics.Random(15000);
        _maxWaitTime = 0.25f;
        _currentWaitTime = _maxWaitTime;
        _lastCachedPlayerMovementDirection = float3.zero;
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.TryGetSingletonRW(out RefRW<BulletSpawnDataComponent> bulletSpawnDataComponent))
            return;

        if (!SystemAPI.TryGetSingletonEntity<PlayerTagComponent>(out Entity playerEntity))
            return;

        var playerLocalTransform = state.EntityManager.GetComponentData<LocalTransform>(playerEntity);
        var playerMovementComponent = state.EntityManager.GetComponentData<PlayerMovementComponent>(playerEntity);

        if (playerMovementComponent.IsMoving)
            _lastCachedPlayerMovementDirection = playerMovementComponent.MovementVector;


        if (!TryCheckCooldown(ref bulletSpawnDataComponent.ValueRW.SpawnCooldown, SystemAPI.Time.DeltaTime))
            return;

        EntityCommandBuffer entityCommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        if (math.length(_lastCachedPlayerMovementDirection) == 0f)
        {
            float3 randomLinear = _random.NextFloat3Direction();
            randomLinear.z = 0;
            _lastCachedPlayerMovementDirection = randomLinear;
        }

        float3 normalized = math.normalizesafe(_lastCachedPlayerMovementDirection);

        float angle = Vector3.SignedAngle(Vector3.right, normalized, Vector3.forward);

        var createdEntity = entityCommandBuffer.Instantiate(bulletSpawnDataComponent.ValueRO.Prefab);

        entityCommandBuffer.SetComponent(createdEntity, new LocalTransform
        {
            Position = playerLocalTransform.Position,
            Rotation = Quaternion.Euler(new float3(0f, 0f, angle)),
            Scale = 1f,
        });

        entityCommandBuffer.SetComponent(createdEntity, new PhysicsVelocity
        {
            Linear = normalized * 10f,
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
