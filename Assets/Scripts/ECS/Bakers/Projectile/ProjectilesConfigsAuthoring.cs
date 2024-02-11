using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ProjectilesConfigsAuthoring : MonoBehaviour
{
    [SerializeField]
    GameObject _bulletPrefab;

    [SerializeField]
    GameObject _rocketPrefab;

    [SerializeField]
    GameObject _lavaPrefab;

    class Baker : Baker<ProjectilesConfigsAuthoring>
    {
        public override void Bake(ProjectilesConfigsAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new BulletSpawnDataComponent
            {
                Prefab = GetEntity(authoring._bulletPrefab, TransformUsageFlags.Dynamic),
            });

            AddComponent(entity, new RocketSpawnDataComponent
            {
                Prefab = GetEntity(authoring._rocketPrefab, TransformUsageFlags.Dynamic),
            });

            AddComponent(entity, new LavaSpawnDataComponent
            {
                Prefab = GetEntity(authoring._lavaPrefab, TransformUsageFlags.Dynamic),
            });
        }
    }
}
