using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class MassCenterPullSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float3 centerOfMassValues = new float3(0, 0, 0);
        Entities.ForEach((in MoveData moveData, in Translation translation) =>
        {
            centerOfMassValues.x += translation.Value.x;
            centerOfMassValues.y += translation.Value.y;
            centerOfMassValues.z += 1;
        }).Run();
        centerOfMassValues.x /= centerOfMassValues.z;
        centerOfMassValues.y /= centerOfMassValues.z;
        centerOfMassValues.z = 0;

        Entities.ForEach((ref MoveData moveData, in Translation translation) =>
        {
            moveData.velocityDelta += math.normalize(centerOfMassValues - translation.Value) * moveData.centerPullMultiplier;
        }).ScheduleParallel();
    }
}
