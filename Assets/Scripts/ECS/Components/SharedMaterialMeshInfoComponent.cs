using Unity.Entities;
using Unity.Rendering;

public struct SharedMaterialMeshInfoComponent : ISharedComponentData
{
    public int Id;
    public MaterialMeshInfo Info;
}
