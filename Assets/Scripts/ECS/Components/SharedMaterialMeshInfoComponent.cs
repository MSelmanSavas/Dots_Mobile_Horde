using Unity.Entities;
using Unity.Rendering;

public struct SharedMaterialMeshInfoComponent : ISharedComponentData
{
    public MaterialMeshInfo Info;
}
