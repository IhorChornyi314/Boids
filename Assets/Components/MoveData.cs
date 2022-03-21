using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]
public struct MoveData : IComponentData
{
    public float3 velocityDelta;
    public float centerPullMultiplier, distanceMaintainMultiplier, velocityAlignMultiplier;
    public float turningSpeed, movementSpeed;
    public float collisionDetectionRadius;
    public float bound;
}
