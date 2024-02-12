using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Serialization;
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
            data.EnemyMesh = CreateQuadMesh(3, 3);
        }
    }

    Mesh CreateQuadMesh(float width, float height)
    {
        Mesh mesh = new Mesh();

        var vertices = new Vector3[4];

        vertices[0] = new Vector3(0, 0, 0);
        vertices[1] = new Vector3(width, 0, 0);
        vertices[2] = new Vector3(0, height, 0);
        vertices[3] = new Vector3(width, height, 0);

        mesh.vertices = vertices;

        var tri = new int[6];

        tri[0] = 0;
        tri[1] = 2;
        tri[2] = 1;

        tri[3] = 2;
        tri[4] = 3;
        tri[5] = 1;

        mesh.triangles = tri;

        var normals = new Vector3[4];

        normals[0] = -Vector3.forward;
        normals[1] = -Vector3.forward;
        normals[2] = -Vector3.forward;
        normals[3] = -Vector3.forward;

        mesh.normals = normals;

        var uv = new Vector2[4];

        uv[0] = new Vector2(0, 0);
        uv[1] = new Vector2(1, 0);
        uv[2] = new Vector2(0, 1);
        uv[3] = new Vector2(1, 1);

        mesh.uv = uv;

        return mesh;
    }

    public List<EnemySpawnerDataGameobject> EnemySpawnerDatas = new();

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
            var entity = GetEntity(TransformUsageFlags.None);
            var buffer = AddBuffer<EnemySpawnerDataComponent>(entity).Reinterpret<EnemySpawnerDataComponent>();

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
