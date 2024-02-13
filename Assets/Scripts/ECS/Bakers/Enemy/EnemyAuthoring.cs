using Unity.Entities;
using Unity.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

public class EnemyAuthoring : MonoBehaviour
{
    [SerializeField]
    EntityComponent_Health entityComponent_Health;

    [SerializeField]
    EnemyAttackComponent _enemyAttackData;

    [SerializeField]
    EnemySpeedComponent _enemySpeedData;

    class Baker : Baker<EnemyAuthoring>
    {
        public override void Bake(EnemyAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new EnemyTagComponent { });
            AddComponent(entity, authoring.entityComponent_Health.GetFullHealth());
            AddComponent(entity, authoring._enemyAttackData);
            AddComponent(entity, authoring._enemySpeedData);
        }
    }
}
