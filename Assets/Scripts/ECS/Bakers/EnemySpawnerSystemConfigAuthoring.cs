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
    public static EnemySpawnerSystemConfigAuthoring Instance;

    private void Awake()
    {
        Instance = this;

        for (int i = 0; i < EnemySpawnerDatas.Count; i++)
        {
            var data = EnemySpawnerDatas[i];
            //data.EnemyMesh = CreateQuadMesh(3, 3);
        }
    }



    [Sirenix.OdinInspector.Button]
    void ForceSetSameMeshToDatas(float2 size, float2 pivot)
    {
        Mesh mesh = MeshUtils.CreateQuadMesh(new float2(size), pivot);

        foreach (var data in EnemySpawnerDatas)
        {
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
        public Material EnemyMaterial;
        public int CurrentAmountSpawned;
        public int MaxAmountSpawned;
    }

    class Baker : Baker<EnemySpawnerSystemConfigAuthoring>
    {
        public override void Bake(EnemySpawnerSystemConfigAuthoring authoring)
        {
            authoring.ForceSetSameMeshToDatas(new float2(2), new float2(0.5));

            var entity = GetEntity(TransformUsageFlags.None);
            var buffer = AddBuffer<EnemySpawnerDataComponent>(entity);

            AddComponentObject(entity, new EnemySpawnerRenderMeshesAndMaterialsComponent
            {
                Meshes = authoring.GetMeshes(),
                Materials = authoring.GetMaterials(),
            });

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
                });
            }
        }
    }
}
