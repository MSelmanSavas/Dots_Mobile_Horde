using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Rendering;
using System.Collections.Generic;
using UnityEngine.Rendering;
using Unity.Collections;
using System.Linq;


[UpdateAfter(typeof(PlayerMovementSyncEntitySystem))]
public partial class EnemySpawnSystem : SystemBase
{
    EntityCommandBuffer _entityCommandBuffer;
    DynamicBuffer<EnemySpawnerDataComponent> _enemyDatas;
    float _currentWaitTime;
    float _maxWaitTime;

    RenderMeshDescription _renderMeshDescription;
    RenderMeshArray _renderMeshArray;
    protected override void OnCreate()
    {
        base.OnCreate();
        _maxWaitTime = 0.25f;
        _currentWaitTime = _maxWaitTime;
    }

    protected override void OnStartRunning()
    {
        base.OnStartRunning();

        if (!SystemAPI.TryGetSingletonBuffer(out _enemyDatas))
            return;

        if (!SystemAPI.ManagedAPI.TryGetSingleton(out EnemySpawnerRenderMeshesAndMaterialsComponent enemySpawnerRenderMeshes))
            return;

        _renderMeshArray = RenderMeshArray.CreateWithDeduplication(enemySpawnerRenderMeshes.Materials, enemySpawnerRenderMeshes.Meshes); ;

        NativeList<Entity> entities = new NativeList<Entity>(Allocator.Temp);

        for (int i = 0; i < _enemyDatas.Length; i++)
        {
            var data = _enemyDatas[i];

            entities.Add(data.Prefab);
        }

        for (int i = 0; i < entities.Length; i++)
        {
            var entity = entities[i];

            int materialIndex = System.Array.IndexOf(_renderMeshArray.Materials, enemySpawnerRenderMeshes.Materials[i]);
            int meshIndex = System.Array.IndexOf(_renderMeshArray.Meshes, enemySpawnerRenderMeshes.Meshes[i]);

            EntityManager.AddComponent<MaterialMeshInfo>(entity);
            EntityManager.SetComponentData(entity, MaterialMeshInfo.FromRenderMeshArrayIndices(materialIndex, meshIndex));
            EntityManager.AddSharedComponentManaged(entity, _renderMeshArray);
        }

        entities.Dispose();
    }


    protected override void OnUpdate()
    {
        if (!TryCheckCooldown(SystemAPI.Time.DeltaTime))
            return;

        if (!SystemAPI.TryGetSingletonBuffer(out _enemyDatas))
            return;

        if (!SystemAPI.TryGetSingletonEntity<PlayerTagComponent>(out Entity playerEntity))
            return;

        PlayerAspect playerAspect = EntityManager.GetAspect<PlayerAspect>(playerEntity);

        _entityCommandBuffer = World.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>().CreateCommandBuffer();

        for (int i = 0; i < _enemyDatas.Length; i++)
        {
            var data = _enemyDatas[i];

            if (data.CurrentAmountSpawned >= data.MaxAmountSpawned)
                continue;

            var createdEntity = _entityCommandBuffer.Instantiate(data.Prefab);

            float3 randomPosition = UnityEngine.Random.insideUnitSphere;
            randomPosition.z = 0;
            randomPosition = math.normalizesafe(randomPosition) * 30;

            randomPosition += playerAspect.PlayerTransform.ValueRO.Position;

            _entityCommandBuffer.SetComponent(createdEntity, new LocalTransform
            {
                Position = randomPosition,
                Rotation = Quaternion.identity,
                Scale = 1f,
            });

            _entityCommandBuffer.SetComponent(createdEntity, new EnemyTagComponent
            {
                EnemySpawnerDataIndex = data.EntitySpawnerDataIndex,
            });

            //_entityCommandBuffer.SetComponent(createdEntity, MaterialMeshInfo.FromRenderMeshArrayIndices(data.EntitySpawnerDataIndex, data.EntitySpawnerDataIndex));

            data.CurrentAmountSpawned = data.CurrentAmountSpawned + 1;

            _enemyDatas[i] = data;
        }
    }

    bool TryCheckCooldown(float deltaTime)
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
