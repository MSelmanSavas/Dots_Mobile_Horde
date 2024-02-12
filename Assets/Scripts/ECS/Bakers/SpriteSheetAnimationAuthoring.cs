using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class SpriteSheetAnimationAuthoring : MonoBehaviour
{
    [SerializeField]
    SpriteSheetAnimationComponent spriteSheetAnimationComponent;

    class Baker : Baker<SpriteSheetAnimationAuthoring>
    {
        public override void Bake(SpriteSheetAnimationAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, authoring.spriteSheetAnimationComponent);
        }
    }
}
