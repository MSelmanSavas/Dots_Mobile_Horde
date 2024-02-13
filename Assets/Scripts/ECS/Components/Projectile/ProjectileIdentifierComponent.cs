using Unity.Collections;
using Unity.Entities;

[System.Serializable]
public struct ProjectileIdentifierComponent : IComponentData
{
    public int Identifier;
}
