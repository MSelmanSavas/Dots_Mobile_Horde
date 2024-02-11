using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class PlayerMovementSyncEntitySystem : SystemBase
{
    Vector2 _screenCenterPos;

    protected override void OnCreate()
    {
        base.OnCreate();
        _screenCenterPos = new()
        {
            x = Screen.width / 2f,
            y = Screen.height / 2f,
        };
    }
    protected override void OnUpdate()
    {
        Entities.WithAll<PlayerAspect>().ForEach((PlayerAspect playerAspect, PlayerControllerComponent playerController) =>
        {
            bool isMoving = GetPlayerInput(out float3 inputVector);

            if (!isMoving)
            {
                playerAspect.PlayerMovement.ValueRW.IsMoving = false;
                playerAspect.PlayerMovement.ValueRW.MovementVector = float3.zero;
                return;
            }

            playerAspect.PlayerMovement.ValueRW.IsMoving = true;
            playerAspect.PlayerMovement.ValueRW.MovementVector = inputVector;
            playerAspect.PlayerTransform.ValueRW.Position = playerAspect.PlayerTransform.ValueRW.Position + (playerAspect.PlayerMovementConfig.ValueRO.Speed * inputVector * SystemAPI.Time.DeltaTime);
            playerController.PlayerController.transform.position = playerAspect.PlayerTransform.ValueRO.Position;
        }).WithoutBurst().Run();
    }

    bool GetPlayerInput(out float3 inputVector)
    {
        inputVector = float3.zero;

        if (Application.isMobilePlatform)
            return TryGetTouchInput(out inputVector);

        return TryGetKeyboardInput(out inputVector);
    }

    bool TryGetKeyboardInput(out float3 inputVector)
    {
        inputVector = float3.zero;
        bool IsMoving = Input.GetKey(KeyCode.LeftArrow)
                        || Input.GetKey(KeyCode.UpArrow)
                        || Input.GetKey(KeyCode.RightArrow)
                        || Input.GetKey(KeyCode.DownArrow);

        if (!IsMoving)
            return false;

        inputVector = Convert.ToInt32(Input.GetKey(KeyCode.LeftArrow)) * Vector3.left
                            + Convert.ToInt32(Input.GetKey(KeyCode.UpArrow)) * Vector3.up
                            + Convert.ToInt32(Input.GetKey(KeyCode.RightArrow)) * Vector3.right
                            + Convert.ToInt32(Input.GetKey(KeyCode.DownArrow)) * Vector3.down;

        return true;
    }

    bool TryGetTouchInput(out float3 inputVector)
    {
        inputVector = float3.zero;

        if (Input.touchCount <= 0)
            return false;

        Vector2 touchPosition = Input.touches[0].position;
        Vector2 deltaPosition = touchPosition - _screenCenterPos;
        float3 movementVector = new float3();

        movementVector.z = 0;
        movementVector.x = deltaPosition.x;
        movementVector.y = deltaPosition.y;

        inputVector = math.normalizesafe(movementVector);
        return true;
    }
}
