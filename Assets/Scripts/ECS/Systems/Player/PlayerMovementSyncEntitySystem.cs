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
        UnityEngine.Profiling.Profiler.BeginSample("Test1");
        var playerEntity = GetEntityQuery(typeof(PlayerTagComponent)).GetSingletonEntity();
        UnityEngine.Profiling.Profiler.EndSample();

        UnityEngine.Profiling.Profiler.BeginSample("Test2");
        var playerTagComponent = EntityManager.GetComponentData<PlayerTagComponent>(playerEntity);
        var localTransform = EntityManager.GetComponentData<LocalTransform>(playerEntity);
        var playerMovementConfigComponent = EntityManager.GetComponentData<PlayerMovementConfigComponent>(playerEntity);
        UnityEngine.Profiling.Profiler.EndSample();

        UnityEngine.Profiling.Profiler.BeginSample("Test3");
        float3 inputVector = Convert.ToInt32(Input.GetKey(KeyCode.LeftArrow)) * Vector3.left
          + Convert.ToInt32(Input.GetKey(KeyCode.UpArrow)) * Vector3.up
          + Convert.ToInt32(Input.GetKey(KeyCode.RightArrow)) * Vector3.right
          + Convert.ToInt32(Input.GetKey(KeyCode.DownArrow)) * Vector3.down;
        UnityEngine.Profiling.Profiler.EndSample();

        UnityEngine.Profiling.Profiler.BeginSample("Test4");
        localTransform.Position = localTransform.Position + (playerMovementConfigComponent.Speed * inputVector * SystemAPI.Time.DeltaTime);
        playerTagComponent.PlayerController.transform.position = localTransform.Position;
        UnityEngine.Profiling.Profiler.EndSample();

    }
}
