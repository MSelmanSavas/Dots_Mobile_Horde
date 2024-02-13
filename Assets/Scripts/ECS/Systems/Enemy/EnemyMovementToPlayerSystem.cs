using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct EnemyMovementToPlayerSystem : ISystem
{
    bool _applyImpulse;

    public void OnCreate(ref SystemState systemState)
    {
        _applyImpulse = false;
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState systemState)
    {
        if (!SystemAPI.TryGetSingletonEntity<PlayerTagComponent>(out Entity playerEntity))
            return;

        float3 playerPosition = systemState.EntityManager.GetComponentData<LocalTransform>(playerEntity).Position;

        float deltaTime = SystemAPI.Time.DeltaTime;

        if (_applyImpulse)
        {
            systemState.Dependency = new MoveWithImpulseJob
            {
                PlayerPosition = playerPosition,
                DeltaTime = deltaTime,
            }.ScheduleParallel(systemState.Dependency);
        }
        else
        {
            systemState.Dependency = new MoveWithVelocityJob
            {
                PlayerPosition = playerPosition,
                DeltaTime = deltaTime,
            }.ScheduleParallel(systemState.Dependency);
        }

        return;
    }

    [BurstCompile]
    partial struct MoveWithImpulseJob : IJobEntity
    {
        public float DeltaTime;
        public float3 PlayerPosition;
        void Execute(ref PhysicsVelocity physicsVelocity, ref PhysicsMass physicsMass, in LocalTransform localTransform, in EnemySpeedComponent enemySpeed)
        {
            float3 enemyPosition = localTransform.Position;
            float3 enemyToPlayerVector = PlayerPosition - enemyPosition;
            physicsVelocity.ApplyLinearImpulse(physicsMass, enemyToPlayerVector * enemySpeed.Speed * DeltaTime);
            Vector3 linearVelocity = physicsVelocity.Linear;
            float velocityScale = linearVelocity.magnitude;
            float clampedVelocityMagnitude = Mathf.Clamp(velocityScale, 0f, 10f);
            physicsVelocity.Linear = linearVelocity.normalized * clampedVelocityMagnitude;
            physicsVelocity.Angular = 0f;
            physicsMass.InverseInertia = float3.zero;
        }
    }

    [BurstCompile]
    partial struct MoveWithVelocityJob : IJobEntity
    {
        public float DeltaTime;
        public float3 PlayerPosition;
        void Execute(ref PhysicsVelocity physicsVelocity, ref PhysicsMass physicsMass, in LocalTransform localTransform, in EnemySpeedComponent enemySpeed)
        {
            float3 enemyPosition = localTransform.Position;
            float3 enemyToPlayerVector = PlayerPosition - enemyPosition;
            float3 directionVector = math.normalizesafe(enemyToPlayerVector);
            physicsVelocity.Linear = directionVector * enemySpeed.Speed;
            physicsVelocity.Angular = 0f;
            physicsMass.InverseInertia = float3.zero;
        }
    }
}
