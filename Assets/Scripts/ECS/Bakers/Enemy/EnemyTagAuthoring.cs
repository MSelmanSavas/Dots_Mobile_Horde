using Unity.Entities;
using UnityEngine;

public class EnemyTagAuthoring : MonoBehaviour
{
    class Baker : Baker<EnemyTagAuthoring>
    {
        public override void Bake(EnemyTagAuthoring authoring)
        {
            AddComponent(GetEntity(TransformUsageFlags.None), new EnemyTagComponent { });
        }
    }
}
