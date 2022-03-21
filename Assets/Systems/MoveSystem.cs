using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

public class MoveSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        Entities.ForEach((ref Translation translation, ref MoveData moveData, ref Rotation rotation, ref DirectionComponent direction) =>
            {
                if (translation.Value.x > moveData.bound * 2)
                {
                    moveData.velocityDelta.x = -5 * math.abs(moveData.velocityDelta.x);
                }
                if (translation.Value.x < -moveData.bound * 2)
                {
                    moveData.velocityDelta.x = 5 * math.abs(moveData.velocityDelta.x);
                }
                if (translation.Value.y > moveData.bound)
                {
                    moveData.velocityDelta.y = -5 * math.abs(moveData.velocityDelta.y);
                }
                if (translation.Value.y < -moveData.bound)
                {
                    moveData.velocityDelta.y = 5 * math.abs(moveData.velocityDelta.y);
                }
                
                float speed = math.distance(moveData.velocityDelta, float3.zero) + 0.01f;
                float3 newDir = moveData.velocityDelta / speed;
                
                direction.direction = math.lerp(direction.direction, newDir, moveData.turningSpeed * deltaTime);
                float3 displacement = direction.direction * deltaTime * moveData.movementSpeed;
                
                rotation.Value = quaternion.RotateZ(math.atan2(direction.direction.y, direction.direction.x) - math.PI / 2);
                translation.Value += displacement;
                moveData.velocityDelta = float3.zero;
            }).ScheduleParallel();
    }
}
