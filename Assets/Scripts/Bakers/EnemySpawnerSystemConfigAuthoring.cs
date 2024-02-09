using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Serialization;
using Unity.Scenes;
using UnityEngine;

public class EnemySpawnerSystemConfigAuthoring : MonoBehaviour
{
    public List<EnemySpawnerDataGameobject> EnemySpawnerDatas = new();

    [System.Serializable]
    public struct EnemySpawnerDataGameobject
    {
        public GameObject Prefab;
        public int CurrentAmountSpawned;
        public int MaxAmountSpawned;
    }

    class Baker : Baker<EnemySpawnerSystemConfigAuthoring>
    {
        public override void Bake(EnemySpawnerSystemConfigAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            var buffer = AddBuffer<EnemySpawnerData>(entity).Reinterpret<EnemySpawnerData>();

            for (int i = 0; i < authoring.EnemySpawnerDatas.Count; i++)
            {
                var prefabEntity = GetEntity(authoring.EnemySpawnerDatas[i].Prefab, TransformUsageFlags.Dynamic);

                buffer.Add(new EnemySpawnerData
                {
                    Prefab = prefabEntity,
                    CurrentAmountSpawned = authoring.EnemySpawnerDatas[i].CurrentAmountSpawned,
                    MaxAmountSpawned = authoring.EnemySpawnerDatas[i].MaxAmountSpawned,
                });
            }
        }
    }
}
