using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

public class ProjectilesConfigsAuthoring : MonoBehaviour
{
    #region Bullet Variables
    [Header("Bullet Datas")]
    [SerializeField]
    GameObject _bulletPrefab;

    [SerializeField]
    Vector2 _bulletMeshSize;

    [SerializeField]
    Material _bulletMaterial;

    [SerializeField]
    ProjectileSpawnCooldownComponent _bulletSpawnCooldown;
    #endregion

    #region Rocket Variables
    [Header("Rocket Datas")]
    [SerializeField]
    GameObject _rocketPrefab;

    [SerializeField]
    Vector2 _rocketMeshSize;

    [SerializeField]
    Material _rocketMaterial;

    [SerializeField]
    ProjectileSpawnCooldownComponent _rocketSpawnCooldown;
    #endregion

    #region Lava Variables
    [Header("Lava Datas")]
    [SerializeField]
    GameObject _lavaPrefab;

    [SerializeField]
    Material _lavaMaterial;

    [SerializeField]
    Vector2 _lavaMeshSize;
    #endregion

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
                SpawnCooldown = authoring._bulletSpawnCooldown,
            });

            AddComponent(entity, new RocketSpawnDataComponent
            {
                Prefab = rocketEntityPrefab,
                SpawnCooldown = authoring._rocketSpawnCooldown,
            });

            AddComponent(entity, new LavaSpawnDataComponent
            {
                Prefab = lavaEntityPrefab,
            });

            List<Material> materials = new()
            {
                authoring._bulletMaterial,
                authoring._rocketMaterial,
                authoring._lavaMaterial,
            };

            List<Mesh> meshes = new()
            {
               MeshUtils.CreateQuadMesh(authoring._bulletMeshSize, new float2(0.5f)),
               MeshUtils.CreateQuadMesh(authoring._rocketMeshSize, new float2(0.5f)),
               MeshUtils.CreateQuadMesh(authoring._lavaMeshSize, new float2(0.5f)),
            };

            AddComponentObject<ProjectilesRenderDatasSharedComponent>(entity, new ProjectilesRenderDatasSharedComponent
            {
                Materials = materials,
                Meshes = meshes,
                RenderMeshArray = RenderMeshArray.CreateWithDeduplication(materials, meshes),
            });
        }
    }
}
