using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Serialization;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Scenes;
using UnityEngine;
using UnityEngine.Rendering;

public class EnemySpawnerSystemConfigAuthoring : MonoBehaviour
{
    [Sirenix.OdinInspector.Button]
    void ForceSetSameMeshToDatas(float2 size, float2 pivot)
    {
        Mesh mesh = MeshUtils.CreateQuadMesh(new float2(size), pivot);

        foreach (var data in EnemySpawnerDatas)
        {
            data.EnemyMesh = mesh;
        }
    }

    [Sirenix.OdinInspector.Button]
    void ForceSetMeshesToDatas(float2 pivot)
    {
        foreach (var data in EnemySpawnerDatas)
        {
            Mesh mesh = MeshUtils.CreateQuadMesh(data.MeshSize, pivot);
            data.EnemyMesh = mesh;
        }
    }

    public List<EnemySpawnerDataGameobject> EnemySpawnerDatas = new();

    List<Mesh> GetMeshes()
    {
        return EnemySpawnerDatas.Select(x => x.EnemyMesh).ToList();
    }

    List<Material> GetMaterials()
    {
        return EnemySpawnerDatas.Select(x => x.EnemyMaterial).ToList();
    }

    [System.Serializable]
    public class EnemySpawnerDataGameobject
    {
        public GameObject Prefab;
        public Mesh EnemyMesh;
        public Vector2 MeshSize;
        public Material EnemyMaterial;
        public int CurrentAmountSpawned;
        public int MaxAmountSpawned;
        public GenericCooldownComponent CooldownComponent;
    }

    class Baker : Baker<EnemySpawnerSystemConfigAuthoring>
    {
        public override void Bake(EnemySpawnerSystemConfigAuthoring authoring)
        {
            for (int i = 0; i < authoring.EnemySpawnerDatas.Count; i++)
            {
                DependsOn(authoring.EnemySpawnerDatas[i].Prefab);
            }

            var entity = GetEntity(TransformUsageFlags.None);
            var buffer = AddBuffer<EnemySpawnerDataComponent>(entity);

            for (int i = 0; i < authoring.EnemySpawnerDatas.Count; i++)
            {
                var enemySpawnData = authoring.EnemySpawnerDatas[i];
                var prefabEntity = GetEntity(enemySpawnData.Prefab, TransformUsageFlags.Dynamic);

                buffer.Add(new EnemySpawnerDataComponent
                {
                    Prefab = prefabEntity,
                    EntitySpawnerDataIndex = i,
                    CurrentAmountSpawned = enemySpawnData.CurrentAmountSpawned,
                    MaxAmountSpawned = enemySpawnData.MaxAmountSpawned,
                    GenericCooldown = enemySpawnData.CooldownComponent,
                });
            }

            AddComponentObject(entity, new EnemySpawnerRenderMeshesAndMaterialsComponent
            {
                Meshes = authoring.GetMeshes(),
                Materials = authoring.GetMaterials(),
                RenderMeshArray = RenderMeshArray.CreateWithDeduplication(authoring.GetMaterials(), authoring.GetMeshes()),
            });
        }
    }
}
