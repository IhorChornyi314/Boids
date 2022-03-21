using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class DistanceMaintainSystem : SystemBase
{
    private NativeArray<Translation> birdsCoords;
    protected override void OnCreate()
    {
        base.OnCreate();
        Enabled = true;
    }
    
    protected override void OnUpdate()
    {
        var query = GetEntityQuery(typeof(MoveData), typeof(Translation));

        birdsCoords = query.ToComponentDataArray<Translation>(Allocator.TempJob);
        var quadrants = new NativeMultiHashMap<int2, float3>(birdsCoords.Length, Allocator.TempJob);
        const float quadrantSize = 6;
        for (int i = 0; i < birdsCoords.Length; i++)
        {
            quadrants.Add(new int2((int)(birdsCoords[i].Value.x / quadrantSize), (int)(birdsCoords[i].Value.y / quadrantSize)), birdsCoords[i].Value);
        }
        
        var dep = Entities.WithReadOnly(quadrants).ForEach((ref MoveData moveData, in Translation translation) =>
        {
            float2 direction = new float2();
            int2 ownHash = new int2((int)(translation.Value.x / quadrantSize), (int)(translation.Value.y / quadrantSize));
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if (quadrants.ContainsKey(new int2(ownHash.x + i, ownHash.y + j)))
                    {
                        var mapEnum = quadrants.GetValuesForKey(new int2(ownHash.x + i, ownHash.y + j));
                        do
                        {
                            float distance = math.distance(mapEnum.Current.xy, translation.Value.xy);
                            if (distance < moveData.collisionDetectionRadius)
                            {
                                direction -= (mapEnum.Current.xy - translation.Value.xy) * moveData.collisionDetectionRadius / (distance + 0.1f);
                            }
                        } while (mapEnum.MoveNext());
                    }
                }
            }
            moveData.velocityDelta.x += direction.x * moveData.distanceMaintainMultiplier;
            moveData.velocityDelta.y += direction.y * moveData.distanceMaintainMultiplier;
        }).ScheduleParallel(Dependency);
        dep.Complete();
        birdsCoords.Dispose();
        quadrants.Dispose();
    }
}
