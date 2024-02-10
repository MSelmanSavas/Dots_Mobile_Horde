using Unity.Entities;
using UnityEngine;

public class PlayerTagAuthoring : MonoBehaviour
{
    class Baker : Baker<PlayerTagAuthoring>
    {
        public override void Bake(PlayerTagAuthoring authoring)
        {
            AddComponent(GetEntity(TransformUsageFlags.None), new PlayerTagComponent { });
        }
    }
}
