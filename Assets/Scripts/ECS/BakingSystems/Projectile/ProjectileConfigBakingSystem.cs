using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

[WorldSystemFilter(WorldSystemFilterFlags.BakingSystem)]
public partial class ProjectileConfigBakingSystem : SystemBase
{
    protected override void OnUpdate()
    {
        if (!SystemAPI.TryGetSingleton<BulletSpawnDataComponent>(out BulletSpawnDataComponent bulletSpawnDataComponent))
            return;

        if (!SystemAPI.TryGetSingleton<RocketSpawnDataComponent>(out RocketSpawnDataComponent rocketSpawnDataComponent))
            return;

        if (!SystemAPI.TryGetSingleton<LavaSpawnDataComponent>(out LavaSpawnDataComponent lavaSpawnDataComponent))
            return;

        if (!SystemAPI.ManagedAPI.TryGetSingleton<ProjectilesRenderDatasSharedComponent>(out ProjectilesRenderDatasSharedComponent projectilesRender))
            return;

        projectilesRender.EntityToMaterialMeshInfoIndex.Clear();

        projectilesRender.EntityToMaterialMeshInfoIndex.Add(bulletSpawnDataComponent.Prefab, MaterialMeshInfo.FromRenderMeshArrayIndices(0, 0));
        projectilesRender.EntityToMaterialMeshInfoIndex.Add(rocketSpawnDataComponent.Prefab, MaterialMeshInfo.FromRenderMeshArrayIndices(1, 1));
        projectilesRender.EntityToMaterialMeshInfoIndex.Add(lavaSpawnDataComponent.Prefab, MaterialMeshInfo.FromRenderMeshArrayIndices(2, 2));
    }
}
