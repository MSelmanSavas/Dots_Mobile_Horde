using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

public class ProjectilesConfigsAuthoring : MonoBehaviour
{
    [SerializeField]
    GameObject _bulletPrefab;

    [SerializeField]
    Vector2 _bulletMeshSize;

    [SerializeField]
    Material _bulletMaterial;

    [SerializeField]
    GameObject _rocketPrefab;

    [SerializeField]
    Vector2 _rocketMeshSize;

    [SerializeField]
    Material _rocketMaterial;

    [SerializeField]
    GameObject _lavaPrefab;

    [SerializeField]
    Material _lavaMaterial;

    [SerializeField]
    Vector2 _lavaMeshSize;

    class Baker : Baker<ProjectilesConfigsAuthoring>
    {
        public override void Bake(ProjectilesConfigsAuthoring authoring)
        {
            DependsOn(authoring._bulletPrefab);
            DependsOn(authoring._rocketPrefab);
            DependsOn(authoring._lavaPrefab);

            var entity = GetEntity(TransformUsageFlags.None);

            Entity bulletEntityPrefab = GetEntity(authoring._bulletPrefab, TransformUsageFlags.Dynamic);
            Entity rocketEntityPrefab = GetEntity(authoring._rocketPrefab, TransformUsageFlags.Dynamic);
            Entity lavaEntityPrefab = GetEntity(authoring._lavaPrefab, TransformUsageFlags.Dynamic);

            AddComponent(entity, new BulletSpawnDataComponent
            {
                Prefab = bulletEntityPrefab,
            });

            AddComponent(entity, new RocketSpawnDataComponent
            {
                Prefab = rocketEntityPrefab,
            });

            AddComponent(entity, new LavaSpawnDataComponent
            {
                Prefab = lavaEntityPrefab,
            });

            Dictionary<Entity, MaterialMeshInfo> materialMeshInfos = new();

            materialMeshInfos.Add(bulletEntityPrefab, MaterialMeshInfo.FromRenderMeshArrayIndices(0, 0));
            materialMeshInfos.Add(rocketEntityPrefab, MaterialMeshInfo.FromRenderMeshArrayIndices(1, 1));
            materialMeshInfos.Add(lavaEntityPrefab, MaterialMeshInfo.FromRenderMeshArrayIndices(2, 2));

            AddComponentObject<ProjectilesRenderDatasSharedComponent>(entity, new ProjectilesRenderDatasSharedComponent
            {
                EntityToMaterialMeshInfoIndex = materialMeshInfos,
            });
        }
    }
}
