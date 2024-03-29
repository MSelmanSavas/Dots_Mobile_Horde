using Unity.Entities;
using UnityEngine;

public class RocketAuthoring : MonoBehaviour
{
    [SerializeField]
    ProjectileStartSpeedComponent _projectileStartSpeed;

    class Baker : Baker<RocketAuthoring>
    {
        public override void Bake(RocketAuthoring authoring)
        {
            Entity rocketEntity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(rocketEntity, new RocketTagComponent { });
            AddComponent(rocketEntity, new ProjectileSelfDestructComponent
            {
                PassedTime = 0f,
                TimeToLive = 10f,
            });

            AddComponent(rocketEntity, authoring._projectileStartSpeed);
        }
    }
}
