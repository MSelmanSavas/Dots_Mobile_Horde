using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class BulletsConfigsAuthoring : MonoBehaviour
{
    [SerializeField]
    GameObject _bulletPrefab;

    [SerializeField]
    GameObject _rocketPrefab;

    class Baker : Baker<BulletsConfigsAuthoring>
    {
        public override void Bake(BulletsConfigsAuthoring authoring)
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
        }
    }
}
