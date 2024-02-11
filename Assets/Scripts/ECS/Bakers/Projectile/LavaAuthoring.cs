using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class LavaAuthoring : MonoBehaviour
{
    class Baker : Baker<LavaAuthoring>
    {
        public override void Bake(LavaAuthoring authoring)
        {
            Entity lavaEntity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(lavaEntity, new LavaTagComponent { });
        }
    }
}
