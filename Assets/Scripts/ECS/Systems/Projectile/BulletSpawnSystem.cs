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

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _random = new Unity.Mathematics.Random(15000);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.TryGetSingletonEntity<PlayerTagComponent>(out Entity playerEntity))
            return;

        var playerLocalTransform = state.EntityManager.GetComponentData<LocalTransform>(playerEntity);

        if (!SystemAPI.TryGetSingletonRW(out RefRW<BulletSpawnDataComponent> bulletSpawnDataComponent))
            return;

        if (!TryCheckCooldown(ref bulletSpawnDataComponent.ValueRW.SpawnCooldown, SystemAPI.Time.DeltaTime))
            return;

        float3 randomLinear = _random.NextFloat3Direction();
        randomLinear.z = 0;
        float3 normalized = math.normalizesafe(randomLinear);

        float angle = Vector3.SignedAngle(Vector3.right, normalized, Vector3.forward);

        var createdEntity = state.EntityManager.Instantiate(bulletSpawnDataComponent.ValueRO.Prefab);

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
