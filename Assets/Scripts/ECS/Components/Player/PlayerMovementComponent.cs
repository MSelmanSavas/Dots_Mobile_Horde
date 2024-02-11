using Unity.Entities;
using Unity.Mathematics;

public struct PlayerMovementComponent : IComponentData
{
    public float3 MovementVector;
    public bool IsMoving;
}
