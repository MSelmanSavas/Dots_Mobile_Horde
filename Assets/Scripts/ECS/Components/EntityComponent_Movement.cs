using Unity.Entities;
using Unity.Mathematics;

public struct EntityComponent_Movement : IComponentData
{
    public float3 MoveDirection;
    public float MoveSpeed;
}
