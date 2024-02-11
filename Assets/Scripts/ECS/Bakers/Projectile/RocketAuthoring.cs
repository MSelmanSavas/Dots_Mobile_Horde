using Unity.Entities;
using UnityEngine;

public class RocketAuthoring : MonoBehaviour
{
    class Baker : Baker<RocketAuthoring>
    {
        public override void Bake(RocketAuthoring authoring)
        {
            Entity rocketEntity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(rocketEntity, new RocketTagComponent { });
        }
    }
}
