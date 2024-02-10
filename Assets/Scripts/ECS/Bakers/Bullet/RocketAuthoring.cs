using Unity.Entities;
using UnityEngine;

public class RocketAuthoring : MonoBehaviour
{
    class Baker : Baker<RocketAuthoring>
    {
        public override void Bake(RocketAuthoring authoring)
        {
            Entity bulletEntity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(bulletEntity, new RocketTagComponent { });
        }
    }
}
