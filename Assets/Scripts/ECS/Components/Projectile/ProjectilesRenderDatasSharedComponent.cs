using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

public class ProjectilesRenderDatasSharedComponent : IComponentData
{
    public List<Material> Materials;
    public List<Mesh> Meshes;
    public RenderMeshArray RenderMeshArray;
}
