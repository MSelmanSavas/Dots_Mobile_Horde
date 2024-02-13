using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public static class MeshUtils
{
    public static Mesh CreateQuadMesh(float2 size, float2 pivot)
    {
        float2 scaledPivot = size * pivot;

        Vector3[] _vertices =
        {
            new Vector3(size.x - scaledPivot.x, size.y - scaledPivot.y, 0),
            new Vector3(size.x - scaledPivot.x, -scaledPivot.y, 0),
            new Vector3(-scaledPivot.x, -scaledPivot.y, 0),
            new Vector3(-scaledPivot.x, size.y - scaledPivot.y, 0),
        };

        Vector2[] _uv =
        {
            new Vector2(1, 1),
            new Vector2(1, 0),
            new Vector2(0, 0),
            new Vector2(0, 1)
        };

        int[] triangles =
        {
            0, 1, 2,
            2, 3, 0
        };

        return new Mesh
        {
            vertices = _vertices,
            uv = _uv,
            triangles = triangles
        };
    }
}
