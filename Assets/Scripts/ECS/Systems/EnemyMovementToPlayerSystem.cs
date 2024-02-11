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

        var playerEntity = GetEntityQuery(typeof(PlayerTagComponent)).GetSingletonEntity();
        float3 playerPosition = SystemAPI.GetComponent<LocalTransform>(playerEntity).Position;


        float deltaTime = SystemAPI.Time.DeltaTime;
        float enemySpeed = 2f;


        Entities.WithAll<EnemyTagComponent>().ForEach((ref LocalTransform localTransform, ref PhysicsVelocity physicsVelocity, ref PhysicsMass physicsMass) =>
        {
            float3 enemyPosition = localTransform.Position;
            float3 enemyToPlayerVector = playerPosition - enemyPosition;
            physicsVelocity.ApplyLinearImpulse(physicsMass, enemyToPlayerVector * enemySpeed * deltaTime);
            Vector3 linearVelocity = physicsVelocity.Linear;
            float velocityScale = linearVelocity.magnitude;
            float clampedVelocityMagnitude = Mathf.Clamp(velocityScale, 0f, enemySpeed);
            physicsVelocity.Linear = linearVelocity.normalized * clampedVelocityMagnitude;
            physicsVelocity.Angular = 0f;
            physicsMass.InverseInertia = float3.zero;

        }).WithBurst().ScheduleParallel();

        return;
    }
}
