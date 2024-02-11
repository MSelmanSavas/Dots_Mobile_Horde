using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;
using Unity.Burst;

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
        if (!SystemAPI.TryGetSingleton(out BulletSpawnDataComponent bulletSpawnData))
            return;

        var playerEntity = SystemAPI.GetSingletonEntity<PlayerTagComponent>();
        var playerLocalTransform = state.EntityManager.GetComponentData<LocalTransform>(playerEntity);

        EntityCommandBuffer entityCommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        for (int i = 0; i < 2; i++)
        {
            float3 randomLinear = _random.NextFloat3Direction();
            randomLinear.z = 0;
            float3 normalized = math.normalizesafe(randomLinear);


            var createdEntity = entityCommandBuffer.Instantiate(bulletSpawnData.Prefab);

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
