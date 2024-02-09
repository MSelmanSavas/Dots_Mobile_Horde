using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;

public partial class EnemyDestroySystem : SystemBase
{
    EntityCommandBuffer _entityCommandBuffer;

    float _maxWaitTime = 2f;
    float _currentWaitTime = 0f;
    Unity.Mathematics.Random _random;

    protected override void OnStartRunning()
    {
        base.OnStartRunning();
        _entityCommandBuffer = World.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>().CreateCommandBuffer();
        _random = new Unity.Mathematics.Random(10000);
    }

    protected override void OnUpdate()
    {
        if (!TryCheckCooldown(SystemAPI.Time.DeltaTime))
            return;

        int randomAmount = _random.NextInt(0, 100);
        int currentAmount = 0;

        Entities.ForEach((Entity entity) =>
        {
            if (currentAmount >= randomAmount)
                return;

            int randomCheckAmount = _random.NextInt(10);

            if (randomCheckAmount <= 50)
                return;

            EntityManager.DestroyEntity(entity);

        }).WithoutBurst().WithStructuralChanges().Run();
    }

    private bool TryCheckCooldown(float deltaTime)
    {
        if (_currentWaitTime > 0f)
        {
            _currentWaitTime -= deltaTime;
            return false;
        }

        _currentWaitTime = _maxWaitTime;
        return true;
    }
}
