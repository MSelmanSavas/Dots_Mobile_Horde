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
            float3 inputVector = GetPlayerInput();

            playerAspect.PlayerTransform.ValueRW.Position = playerAspect.PlayerTransform.ValueRW.Position + (playerAspect.PlayerMovementConfig.ValueRO.Speed * inputVector * SystemAPI.Time.DeltaTime);
            playerController.PlayerController.transform.position = playerAspect.PlayerTransform.ValueRO.Position;
        }).WithoutBurst().Run();
    }

    float3 GetPlayerInput()
    {
        if (Application.isMobilePlatform)
            return GetTouchInput();

        return GetKeyboardInput();
    }

    float3 GetKeyboardInput()
    {
        float3 inputVector = Convert.ToInt32(Input.GetKey(KeyCode.LeftArrow)) * Vector3.left
                            + Convert.ToInt32(Input.GetKey(KeyCode.UpArrow)) * Vector3.up
                            + Convert.ToInt32(Input.GetKey(KeyCode.RightArrow)) * Vector3.right
                            + Convert.ToInt32(Input.GetKey(KeyCode.DownArrow)) * Vector3.down;

        return inputVector;
    }

    float3 GetTouchInput()
    {
        if (Input.touchCount <= 0)
            return float3.zero;

        Vector2 touchPosition = Input.touches[0].position;
        Vector2 deltaPosition = touchPosition - Vector2.zero;
        float3 movementVector = new float3();

        movementVector.z = 0;
        movementVector.x = deltaPosition.x;
        movementVector.y = deltaPosition.y;

        movementVector = math.normalizesafe(movementVector);
        return movementVector;
    }
}
