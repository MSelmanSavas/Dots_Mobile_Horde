using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public readonly partial struct PlayerAspect : IAspect
{
    public readonly Entity PlayerEntity;
    public readonly RefRW<EntityComponent_Health> PlayerHealth;
    public readonly RefRO<PlayerTagComponent> PlayerTag;
    public readonly RefRW<LocalTransform> PlayerTransform;
    public readonly RefRW<PlayerMovementComponent> PlayerMovement;
    public readonly RefRO<PlayerMovementConfigComponent> PlayerMovementConfig;
}
