using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

public class EnemySpawnerRenderMeshesAndMaterialsComponent : IComponentData
{
    public List<Mesh> Meshes = new();
    public List<Material> Materials = new();
    public RenderMeshArray RenderMeshArray;
}
