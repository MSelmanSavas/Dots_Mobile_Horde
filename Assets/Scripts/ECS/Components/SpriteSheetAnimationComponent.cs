using System.Numerics;
using Unity.Entities;
using Unity.Mathematics;

[System.Serializable]
public struct SpriteSheetAnimationComponent : IComponentData
{
    public int CurrentFrame;
    public int FrameCount;
    public float FrameTimer;
    public float FrameTimerMax;
    public float4 UV;
    public Matrix4x4 Matrix;
}
