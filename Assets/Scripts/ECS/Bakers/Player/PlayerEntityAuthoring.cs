using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class PlayerEntityAuthoring : MonoBehaviour
{
    class Baker : Baker<PlayerEntityAuthoring>
    {
        public override void Bake(PlayerEntityAuthoring authoring)
        {
            var playerEntity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent<PlayerTagComponent>(playerEntity);

            AddComponent(playerEntity, new PlayerMovementConfigComponent
            {
                Speed = 15,
            });

            AddComponent(playerEntity, new EntityComponent_Health
            {
                CurrentHealth = 100,
                MaxHealth = 100,
                IsDead = false,
            });

            AddComponent<PlayerMovementComponent>(playerEntity);
        }
    }
}
