using Unity.Entities;
using UnityEngine;

public class BulletAuthoring : MonoBehaviour
{
    [SerializeField]
    ProjectileStartSpeedComponent _projectileStartSpeed;

    [SerializeField]
    ProjectileDirectDamageComponent _projectileDirectDamage;

    class Baker : Baker<BulletAuthoring>
    {
        public override void Bake(BulletAuthoring authoring)
        {
            Entity bulletEntity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(bulletEntity, new BulletTagComponent { });

            AddComponent(bulletEntity, new ProjectileSelfDestructComponent
            {
                PassedTime = 0f,
                TimeToLive = 10f,
            });

            AddComponent(bulletEntity, authoring._projectileStartSpeed);
            AddComponent(bulletEntity, authoring._projectileDirectDamage);
        }
    }
}
