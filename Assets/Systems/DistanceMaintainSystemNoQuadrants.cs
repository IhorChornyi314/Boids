using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class DistanceMaintainSystemNoQuadrants : SystemBase
{
    protected override void OnCreate()
    {
        base.OnCreate();
        Enabled = false;
    }
    
    protected override void OnUpdate()
    {
        var query = GetEntityQuery(typeof(MoveData), typeof(Translation));
        NativeArray<Translation> birdsCoords = query.ToComponentDataArray<Translation>(Allocator.TempJob);
        var dep = Entities.ForEach((ref MoveData moveData, in Translation translation) =>
        {
            float2 direction = new float2();
            for (int i = 0; i < birdsCoords.Length; i++)
            {
                if (math.distance(birdsCoords[i].Value.xy, translation.Value.xy) < moveData.collisionDetectionRadius)
                {
                    direction -= (birdsCoords[i].Value.xy - translation.Value.xy);
                }
            }
            moveData.velocityDelta.x += direction.x * moveData.distanceMaintainMultiplier;
            moveData.velocityDelta.y += direction.y * moveData.distanceMaintainMultiplier;
        }).ScheduleParallel(Dependency);
        dep.Complete();
        birdsCoords.Dispose();
    }
}
