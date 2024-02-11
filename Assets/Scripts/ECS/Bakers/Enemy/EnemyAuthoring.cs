using Unity.Entities;
using UnityEngine;

public class EnemyAuthoring : MonoBehaviour
{
    [SerializeField]
    EntityComponent_Health entityComponent_Health;

    class Baker : Baker<EnemyAuthoring>
    {
        public override void Bake(EnemyAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new EnemyTagComponent { });
            AddComponent(entity, authoring.entityComponent_Health.GetFullHealth());
        }
    }
}
