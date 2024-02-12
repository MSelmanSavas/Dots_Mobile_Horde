using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateInGroup(typeof(LateSimulationSystemGroup))]
public partial struct SpriteSheetAnimationSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState systemState)
    {
        SpriteSheetAnimationDataJob job = new SpriteSheetAnimationDataJob
        {
            deltaTime = SystemAPI.Time.DeltaTime
        };

        var spriteSheetDataQuery = SystemAPI.QueryBuilder()
                   .WithAll<SpriteSheetAnimationComponent, LocalTransform>().Build();

        job.ScheduleParallel(spriteSheetDataQuery, systemState.Dependency).Complete();
    }

    [BurstCompile]
    public partial struct SpriteSheetAnimationDataJob : IJobEntity
    {
        public float deltaTime;

        public void Execute(ref SpriteSheetAnimationComponent spriteSheetAnimationData, RefRO<LocalTransform> transform)
        {
            spriteSheetAnimationData.FrameTimer += deltaTime;

            while (spriteSheetAnimationData.FrameTimer >= spriteSheetAnimationData.FrameTimerMax)
            {
                spriteSheetAnimationData.FrameTimer -= spriteSheetAnimationData.FrameTimerMax;
                spriteSheetAnimationData.CurrentFrame = (spriteSheetAnimationData.CurrentFrame + 1) % spriteSheetAnimationData.FrameCount;
            }

            float uvWidth = 1f / spriteSheetAnimationData.FrameCount;
            float uvHeight = 1f;
            float uvOffsetX = uvWidth * spriteSheetAnimationData.CurrentFrame;
            float uvOffsetY = 0f;
            spriteSheetAnimationData.UV = new float4(uvWidth, uvHeight, uvOffsetX, uvOffsetY);
            spriteSheetAnimationData.Matrix = transform.ValueRO.ToMatrix();
        }
    }
}
