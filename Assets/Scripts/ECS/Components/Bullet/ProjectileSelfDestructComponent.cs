using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct ProjectileSelfDestructComponent : IComponentData
{
    public float TimeToLive;
}
