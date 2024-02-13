using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

public class ProjectilesRenderDatasSharedComponent : IComponentData
{
    public Dictionary<Entity, MaterialMeshInfo> EntityToMaterialMeshInfoIndex = new();
    public RenderMeshArray renderMeshArray;
}
