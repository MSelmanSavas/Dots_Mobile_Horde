using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
public partial class PlayerMovementSyncEntitySystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Entity entity, PlayerTagComponent playerTagComponent, ref PlayerMovementConfigComponent playerMovementConfigComponent, ref LocalTransform localTransform) =>
       {

           float3 inputVector = Convert.ToInt32(Input.GetKey(KeyCode.LeftArrow)) * Vector3.left
           + Convert.ToInt32(Input.GetKey(KeyCode.UpArrow)) * Vector3.up
           + Convert.ToInt32(Input.GetKey(KeyCode.RightArrow)) * Vector3.right
           + Convert.ToInt32(Input.GetKey(KeyCode.DownArrow)) * Vector3.down;

           localTransform.Position = localTransform.Position + (playerMovementConfigComponent.Speed * inputVector * SystemAPI.Time.DeltaTime);
           playerTagComponent.PlayerController.transform.position = localTransform.Position;
       }).WithoutBurst().Run();
    }
}
