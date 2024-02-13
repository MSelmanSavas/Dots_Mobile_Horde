using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Transforms;
using UnityEngine;

public partial class EnemyMovementToPlayerSystem : SystemBase
{
    bool _applyImpulse;

    protected override void OnCreate()
    {
        base.OnCreate();
        _applyImpulse = false;
    }

    protected override void OnUpdate()
    {
        if (!SystemAPI.TryGetSingletonEntity<PlayerTagComponent>(out Entity playerEntity))
            return;

        float3 playerPosition = EntityManager.GetComponentData<LocalTransform>(playerEntity).Position;

        float deltaTime = SystemAPI.Time.DeltaTime;
        float enemySpeed = 2f;

        if (_applyImpulse)
        {
            Entities.WithAll<EnemyTagComponent>().ForEach((ref LocalTransform localTransform, ref PhysicsVelocity physicsVelocity, ref PhysicsMass physicsMass) =>
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
        }
        else
        {
            Entities.WithAll<EnemyTagComponent>().ForEach((ref LocalTransform localTransform, ref PhysicsVelocity physicsVelocity, ref PhysicsMass physicsMass) =>
             {
                 float3 enemyPosition = localTransform.Position;
                 float3 enemyToPlayerVector = playerPosition - enemyPosition;
                 float3 directionVector = math.normalizesafe(enemyToPlayerVector);
                 physicsVelocity.Linear = directionVector * enemySpeed;
                 physicsVelocity.Angular = 0f;
                 physicsMass.InverseInertia = float3.zero;

             }).WithBurst().ScheduleParallel();
        }


        return;
    }
}
