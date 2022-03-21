using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class VelocityAlignSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var query = GetEntityQuery(typeof(DirectionComponent));

        NativeArray<DirectionComponent> directionComponents = query.ToComponentDataArray<DirectionComponent>(Allocator.TempJob);
        float3 direction = new float3();
        for (int i = 0; i < directionComponents.Length; i++)
        {
            direction += directionComponents[i].direction;
        }
        var dep = Entities.ForEach((ref MoveData moveData) =>
        {
            moveData.velocityDelta += direction * moveData.velocityAlignMultiplier;
        }).ScheduleParallel(Dependency);
        
        dep.Complete();
        directionComponents.Dispose();
    }
}
