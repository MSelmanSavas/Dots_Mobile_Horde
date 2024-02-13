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
        if (!SystemAPI.TryGetSingleton(out BulletSpawnDataComponent bulletSpawnDataComponent))
            return;

        if (!SystemAPI.TryGetSingleton(out RocketSpawnDataComponent rocketSpawnDataComponent))
            return;

        if (!SystemAPI.TryGetSingleton(out LavaSpawnDataComponent lavaSpawnDataComponent))
            return;

        if (!SystemAPI.ManagedAPI.TryGetSingleton(out ProjectilesRenderDatasSharedComponent projectilesRender))
            return;

        var bulletIdentifier = EntityManager.GetComponentData<ProjectileIdentifierComponent>(bulletSpawnDataComponent.Prefab);
        var rocketIdentifier = EntityManager.GetComponentData<ProjectileIdentifierComponent>(rocketSpawnDataComponent.Prefab);
        var lavaIdentifier = EntityManager.GetComponentData<ProjectileIdentifierComponent>(lavaSpawnDataComponent.Prefab);

        Debug.Log(bulletIdentifier.Identifier);
        Debug.Log(rocketIdentifier.Identifier);
        Debug.Log(lavaIdentifier.Identifier);

        projectilesRender.EntityToMaterialMeshInfoIndex.Clear();

        projectilesRender.EntityToMaterialMeshInfoIndex.Add(bulletSpawnDataComponent.Prefab, MaterialMeshInfo.FromRenderMeshArrayIndices(0, 0));
        projectilesRender.EntityToMaterialMeshInfoIndex.Add(rocketSpawnDataComponent.Prefab, MaterialMeshInfo.FromRenderMeshArrayIndices(1, 1));
        projectilesRender.EntityToMaterialMeshInfoIndex.Add(lavaSpawnDataComponent.Prefab, MaterialMeshInfo.FromRenderMeshArrayIndices(2, 2));
    }
}
