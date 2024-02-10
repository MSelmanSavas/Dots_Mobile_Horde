using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(FixedStepSimulationSystemGroup))]
public partial class RotationalLockSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Entity entity, ref LocalTransform localTransform, ref PhysicsVelocity physicsVelocity) =>
        {
            localTransform.Position = new Unity.Mathematics.float3(localTransform.Position.x, localTransform.Position.y, 0f);
            float rotationDirection = physicsVelocity.Linear.x > 0 ? 0f : 180f;
            localTransform.Rotation = Quaternion.Euler(new Vector3(0f, rotationDirection, 0f));
        }).ScheduleParallel();
    }
}
