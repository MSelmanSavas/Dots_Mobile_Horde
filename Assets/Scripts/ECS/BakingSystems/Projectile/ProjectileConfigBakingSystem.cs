using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

[WorldSystemFilter(WorldSystemFilterFlags.BakingSystem)]
public partial class ProjectileConfigBakingSystem : SystemBase
{
    protected override void OnUpdate()
    {
        if (!SystemAPI.TryGetSingleton(out BulletSpawnDataComponent bulletSpawnDataComponent))
            return;

        if (!SystemAPI.TryGetSingleton(out RocketSpawnDataComponent rocketSpawnDataComponent))
            return;

        if (!SystemAPI.TryGetSingleton(out LavaSpawnDataComponent lavaSpawnDataComponent))
            return;

        if (!SystemAPI.ManagedAPI.TryGetSingleton(out ProjectilesRenderDatasSharedComponent projectilesRender))
            return;

        NativeHashMap<int2, SharedMaterialMeshInfoComponent> materialMeshInfos = new NativeHashMap<int2, SharedMaterialMeshInfoComponent>(20, Allocator.Temp);

        for (int i = 0; i < projectilesRender.RenderMeshArray.Materials.Length; i++)
            for (int j = 0; j < projectilesRender.RenderMeshArray.Meshes.Length; j++)
            {
                materialMeshInfos.Add(new int2(i, j), new SharedMaterialMeshInfoComponent
                {
                    Id = 1,
                    Info = MaterialMeshInfo.FromRenderMeshArrayIndices(i, j),
                });
            }

        NativeList<Entity> entities = new NativeList<Entity>(Allocator.Temp)
        {
            bulletSpawnDataComponent.Prefab,
            rocketSpawnDataComponent.Prefab,
            lavaSpawnDataComponent.Prefab,
        };

        for (int i = 0; i < entities.Length; i++)
        {
            var entity = entities[i];

            int materialIndex = System.Array.IndexOf(projectilesRender.RenderMeshArray.Materials, projectilesRender.Materials[i]);
            int meshIndex = System.Array.IndexOf(projectilesRender.RenderMeshArray.Meshes, projectilesRender.Meshes[i]);

            EntityManager.AddSharedComponent(entity, materialMeshInfos[new int2(materialIndex, meshIndex)]);
            EntityManager.AddSharedComponentManaged(entity, projectilesRender.RenderMeshArray);
        }

        entities.Dispose();
        materialMeshInfos.Dispose();

    }
}
