using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct RotationalLockSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var enemies = SystemAPI.QueryBuilder().WithAll<EnemyTagComponent, LocalTransform, PhysicsVelocity>().Build();

        var enemyRotationLockJob = new RotationalLockJob();

        state.Dependency = enemyRotationLockJob.ScheduleParallel(enemies, state.Dependency);
    }

    [BurstCompile]
    partial struct RotationalLockJob : IJobEntity
    {
        void Execute(ref LocalTransform localTransform, in PhysicsVelocity physicsVelocity)
        {
            localTransform.Position = new Unity.Mathematics.float3(localTransform.Position.x, localTransform.Position.y, 0f);
            float rotationDirection = physicsVelocity.Linear.x > 0 ? 0f : 180f;
            localTransform.Rotation = Quaternion.Euler(new Vector3(0f, rotationDirection, 0f));
        }
    }
}
