using Unity.Entities;
using UnityEngine;

public class ProjectileIdentifierAuthoring : MonoBehaviour
{
    [SerializeField]
    ProjectileIdentifierComponent _projectileIdentifier;

    class Baker : Baker<ProjectileIdentifierAuthoring>
    {
        public override void Bake(ProjectileIdentifierAuthoring authoring)
        {
            Entity projectileEntity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(projectileEntity, authoring._projectileIdentifier);
        }
    }
}
