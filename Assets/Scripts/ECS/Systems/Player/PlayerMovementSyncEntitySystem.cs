using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class PlayerMovementSyncEntitySystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.WithAll<PlayerAspect>().ForEach((PlayerAspect playerAspect, PlayerControllerComponent playerController) =>
        {
            float3 inputVector = Convert.ToInt32(Input.GetKey(KeyCode.LeftArrow)) * Vector3.left
                     + Convert.ToInt32(Input.GetKey(KeyCode.UpArrow)) * Vector3.up
                     + Convert.ToInt32(Input.GetKey(KeyCode.RightArrow)) * Vector3.right
                     + Convert.ToInt32(Input.GetKey(KeyCode.DownArrow)) * Vector3.down;

            playerAspect.PlayerTransform.ValueRW.Position = playerAspect.PlayerTransform.ValueRW.Position + (playerAspect.PlayerMovementConfig.ValueRO.Speed * inputVector * SystemAPI.Time.DeltaTime);
            playerController.PlayerController.transform.position = playerAspect.PlayerTransform.ValueRO.Position;
        }).WithoutBurst().Run();
    }
}
