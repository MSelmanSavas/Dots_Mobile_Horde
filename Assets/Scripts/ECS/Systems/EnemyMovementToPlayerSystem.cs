using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

[UpdateBefore(typeof(FixedStepSimulationSystemGroup))]
public partial class EnemyMovementToPlayerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var playerEntity = GetEntityQuery(typeof(PlayerTagComponent)).GetSingletonEntity();
        float3 playerPosition = SystemAPI.GetComponent<LocalTransform>(playerEntity).Position;

        float deltaTime = SystemAPI.Time.DeltaTime;
        float enemySpeed = 10f;

        Entities.ForEach((Entity entity, ref LocalTransform localTransform, ref PhysicsVelocity physicsVelocity, in EnemyTagComponent enemyTag) =>
        {
            float3 enemyPosition = localTransform.Position;
            float3 enemyToPlayerVector = playerPosition - enemyPosition;
            physicsVelocity.Linear = enemyToPlayerVector * enemySpeed * deltaTime;
        }).WithBurst().ScheduleParallel();

        return;
    }
}
