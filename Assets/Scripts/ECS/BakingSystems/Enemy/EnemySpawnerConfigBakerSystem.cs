using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

[WorldSystemFilter(WorldSystemFilterFlags.BakingSystem)]
public partial class EnemySpawnerConfigBakerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        if (!SystemAPI.TryGetSingletonBuffer(out DynamicBuffer<EnemySpawnerDataComponent> enemyDatas))
            return;

        if (!SystemAPI.ManagedAPI.TryGetSingleton(out EnemySpawnerRenderMeshesAndMaterialsComponent enemyRenderers))
            return;

        NativeHashMap<int2, SharedMaterialMeshInfoComponent> materialMeshInfos = new NativeHashMap<int2, SharedMaterialMeshInfoComponent>(20, Allocator.Temp);

        for (int i = 0; i < enemyRenderers.RenderMeshArray.Materials.Length; i++)
            for (int j = 0; j < enemyRenderers.RenderMeshArray.Meshes.Length; j++)
            {
                materialMeshInfos.Add(new int2(i, j), new SharedMaterialMeshInfoComponent
                {
                    Id = 0,
                    Info = MaterialMeshInfo.FromRenderMeshArrayIndices(i, j),
                });
            }

        NativeList<Entity> entities = new NativeList<Entity>(Allocator.Temp);

        for (int i = 0; i < enemyDatas.Length; i++)
        {
            var data = enemyDatas[i];

            entities.Add(data.Prefab);
        }

        for (int i = 0; i < entities.Length; i++)
        {
            var entity = entities[i];

            int materialIndex = System.Array.IndexOf(enemyRenderers.RenderMeshArray.Materials, enemyRenderers.Materials[i]);
            int meshIndex = System.Array.IndexOf(enemyRenderers.RenderMeshArray.Meshes, enemyRenderers.Meshes[i]);

            EntityManager.AddSharedComponent(entity, materialMeshInfos[new int2(materialIndex, meshIndex)]);
            EntityManager.AddSharedComponentManaged(entity, enemyRenderers.RenderMeshArray);
        }

        entities.Dispose();
        materialMeshInfos.Dispose();
    }
}
