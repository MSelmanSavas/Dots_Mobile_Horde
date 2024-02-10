using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Transforms;
using UnityEngine;

public partial class EnemyMovementToPlayerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        UnityEngine.Profiling.Profiler.BeginSample("Physics Test 1");

        var playerEntity = GetEntityQuery(typeof(PlayerTagComponent)).GetSingletonEntity();
        float3 playerPosition = SystemAPI.GetComponent<LocalTransform>(playerEntity).Position;

        UnityEngine.Profiling.Profiler.EndSample();

        float deltaTime = SystemAPI.Time.DeltaTime;
        float enemySpeed = 5f;

        UnityEngine.Profiling.Profiler.BeginSample("Physics Test 2");

        Entities.WithAll<EnemyTagComponent>().ForEach((Entity entity, ref LocalTransform localTransform, ref PhysicsVelocity physicsVelocity, ref PhysicsMass physicsMass) =>
        {
            float3 enemyPosition = localTransform.Position;
            float3 enemyToPlayerVector = playerPosition - enemyPosition;
            physicsVelocity.ApplyLinearImpulse(physicsMass, enemyToPlayerVector * enemySpeed * deltaTime);
            Vector3 linearVelocity = physicsVelocity.Linear;
            float velocityScale = linearVelocity.magnitude;
            float clampedVelocityMagnitude = Mathf.Clamp(velocityScale, 0f, 10f);
            physicsVelocity.Linear = linearVelocity.normalized * clampedVelocityMagnitude;
            physicsVelocity.Angular = 0f;
            physicsMass.InverseInertia = float3.zero;

        }).WithBurst().ScheduleParallel();

        UnityEngine.Profiling.Profiler.EndSample();

        return;
    }
}
