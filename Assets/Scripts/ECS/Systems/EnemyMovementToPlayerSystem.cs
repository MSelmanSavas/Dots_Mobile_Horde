using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

[UpdateBefore(typeof(FixedStepSimulationSystemGroup))]
public partial class EnemyMovementToPlayerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Entity entity, ref PhysicsVelocity physicsVelocity, in EnemyTagComponent enemyTag) =>
        {
            
        }).ScheduleParallel();
        return;
    }
}
