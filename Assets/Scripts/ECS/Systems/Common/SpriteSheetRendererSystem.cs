using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateAfter(typeof(SpriteSheetAnimationSystem))]
public partial class SpriteSheetRendererSystem : SystemBase
{
    List<RenderMeshArray> renderMeshArrays;
    NativeList<SharedMaterialMeshInfoComponent> materialMeshInfos;
    int shaderPropertyId = Shader.PropertyToID("_MainTex_UV");
    Matrix4x4[] matrixInstancedArray;
    Vector4[] uvInstancedArray;
    Mesh lastFoundMesh;
    Material lastFoundMaterial;
    MaterialPropertyBlock materialPropertyBlock;
    int sliceCount = 1023;

    [BurstCompile]
    protected override void OnStartRunning()
    {
        base.OnStartRunning();
        renderMeshArrays = new(10);
        materialMeshInfos = new(10, Allocator.Persistent);
        matrixInstancedArray = new Matrix4x4[sliceCount];
        uvInstancedArray = new Vector4[sliceCount];
        materialPropertyBlock = new MaterialPropertyBlock();
    }

    [BurstCompile]
    protected override void OnUpdate()
    {
        EntityQuery entityQuery = SystemAPI.QueryBuilder().WithAll<LocalTransform, SpriteSheetAnimationComponent, SharedMaterialMeshInfoComponent, RenderMeshArray>().Build();

        renderMeshArrays.Clear();
        EntityManager.GetAllUniqueSharedComponentsManaged(renderMeshArrays);
        EntityManager.GetAllUniqueSharedComponents(out materialMeshInfos, Allocator.Temp);

        foreach (var renderMeshArray in renderMeshArrays)
        {
            if (renderMeshArray == null)
                continue;

            if (renderMeshArray.Meshes == null || renderMeshArray.Materials == null)
                continue;


            foreach (var materialMeshInfo in materialMeshInfos)
            {
                if (materialMeshInfo.Info.Mesh == 0 || materialMeshInfo.Info.Material == 0)
                    continue;

                entityQuery.ResetFilter();
                entityQuery.AddSharedComponentFilterManaged(renderMeshArray);
                entityQuery.AddSharedComponentFilter(materialMeshInfo);

                int entityCount = entityQuery.CalculateEntityCount();

                if (entityCount <= 0)
                    continue;

                NativeArray<SpriteSheetAnimationComponent> animationComponents = entityQuery.ToComponentDataArray<SpriteSheetAnimationComponent>(Allocator.TempJob);

                lastFoundMesh = renderMeshArray.GetMesh(materialMeshInfo.Info);
                lastFoundMaterial = renderMeshArray.GetMaterial(materialMeshInfo.Info);

                NativeArray<Matrix4x4> matrixArray = new NativeArray<Matrix4x4>(animationComponents.Length, Allocator.TempJob);
                NativeArray<Vector4> uvArray = new NativeArray<Vector4>(animationComponents.Length, Allocator.TempJob);

                new FillArraysParallelJob
                {
                    nativeArray = animationComponents.AsReadOnly(),
                    matrixArray = matrixArray,
                    uvArray = uvArray,
                }.Schedule(animationComponents.Length, 10).Complete();

                for (int i = 0; i < animationComponents.Length; i += sliceCount)
                {
                    int sliceSize = math.min(animationComponents.Length - i, sliceCount);

                    NativeArray<Matrix4x4>.Copy(matrixArray, i, matrixInstancedArray, 0, sliceSize);
                    NativeArray<Vector4>.Copy(uvArray, i, uvInstancedArray, 0, sliceSize);

                    materialPropertyBlock.SetVectorArray(shaderPropertyId, uvInstancedArray);

                    Graphics.DrawMeshInstanced(
                        lastFoundMesh,
                        0,
                        lastFoundMaterial,
                        matrixInstancedArray,
                        sliceSize,
                        materialPropertyBlock
                    );
                }

                matrixArray.Dispose();
                uvArray.Dispose();
                animationComponents.Dispose();
            }
        }
    }

    [BurstCompile]
    private struct FillArraysParallelJob : IJobParallelFor
    {
        public NativeArray<SpriteSheetAnimationComponent>.ReadOnly nativeArray;
        public NativeArray<Matrix4x4> matrixArray;
        public NativeArray<Vector4> uvArray;

        public void Execute(int index)
        {
            SpriteSheetAnimationComponent entityPositionWithIndex = nativeArray[index];
            matrixArray[index] = entityPositionWithIndex.Matrix;
            uvArray[index] = entityPositionWithIndex.UV;
        }
    }
}
