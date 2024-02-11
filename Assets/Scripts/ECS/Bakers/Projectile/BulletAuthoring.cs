using Unity.Entities;
using UnityEngine;

public class BulletAuthoring : MonoBehaviour
{
    class Baker : Baker<BulletAuthoring>
    {
        public override void Bake(BulletAuthoring authoring)
        {
            Entity bulletEntity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(bulletEntity, new BulletTagComponent { });
        }
    }
}
