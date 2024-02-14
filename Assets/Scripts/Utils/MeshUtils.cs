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

    public static Mesh CreateQuadMesh(int width, int height, float unitWidthLength, float unitHeightLength, bool isHorizontal)
    {
        var halfWidth = width * unitWidthLength / 2f;
        var halfHeight = height * unitHeightLength / 2f;
        var positionDelta =
            isHorizontal ? new Vector3(-halfWidth, 0, -halfHeight) : new Vector3(-halfWidth, -halfHeight);
        width++;
        height++;
        var direction = isHorizontal ? Vector3.forward : Vector3.up;
        var verticesCount = width * height;
        var triangleCount = (width - 1) * (height - 1) * 2;
        var vertices = new Vector3[verticesCount];
        var uvs = new Vector2[verticesCount];
        var triangles = new int[triangleCount * 3];
        var trisIndex = 0;
        for (var w = 0; w < width; w++)
        {
            for (var h = 0; h < height; h++)
            {
                var vertIndex = h * width + w;
                var position = Vector3.right * w * unitWidthLength + direction * h * unitHeightLength;
                vertices[vertIndex] = position + positionDelta;
                uvs[vertIndex] = new Vector2(w / (width - 1f), h / (height - 1f));
                if (w == width - 1 || h == height - 1)
                {
                    continue;
                }

                triangles[trisIndex++] = vertIndex;
                triangles[trisIndex++] = vertIndex + width;
                triangles[trisIndex++] = vertIndex + width + 1;
                triangles[trisIndex++] = vertIndex;
                triangles[trisIndex++] = vertIndex + width + 1;
                triangles[trisIndex++] = vertIndex + 1;
            }
        }

        var mesh = new Mesh { vertices = vertices, triangles = triangles, uv = uvs };
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        return mesh;
    }
}
