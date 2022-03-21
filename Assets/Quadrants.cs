using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public static class Quadrants
{
    public static int CalculateHash(float3 position, float quadrantSize, int numberByX)
    {
        int hash = (int) (position.x / quadrantSize) + numberByX * (int) (position.y / quadrantSize);
        return hash;
    }
    
    public static void GetQuadrants(ref NativeMultiHashMap<int, float3> result, in NativeArray<Translation> positions,
        float quadrantSize, float maxX)
    {
        int numberByX = (int)(maxX / quadrantSize);
        for (int i = 0; i < positions.Length; i++)
        {
            result.Add(CalculateHash(positions[i].Value, quadrantSize, numberByX), positions[i].Value);
        }
    }

    public static void GetCoordsFromQuadrant(ref NativeList<float3> result, NativeMultiHashMap<int, float3> map, int hash)
    {
        if (map.ContainsKey(hash))
        {
            var mapEnum = map.GetValuesForKey(hash);
            do
            {
                result.Add(mapEnum.Current);
            } while (mapEnum.MoveNext());
        }
    }
    
    public static NativeList<float3> GetNearbyCoords(ref NativeList<float3> result, NativeMultiHashMap<int, float3> map, float3 startCoords,
        float quadrantSize, int numberByX)
    {
        int ownHash = CalculateHash(startCoords, quadrantSize, numberByX);
        GetCoordsFromQuadrant(ref result, map, ownHash);
        GetCoordsFromQuadrant(ref result, map, ownHash - 1);
        GetCoordsFromQuadrant(ref result, map, ownHash + 1);
        GetCoordsFromQuadrant(ref result, map, ownHash - numberByX);
        GetCoordsFromQuadrant(ref result, map, ownHash - numberByX - 1);
        GetCoordsFromQuadrant(ref result, map, ownHash - numberByX + 1);
        GetCoordsFromQuadrant(ref result, map, ownHash + numberByX);
        GetCoordsFromQuadrant(ref result, map, ownHash + numberByX - 1);
        GetCoordsFromQuadrant(ref result, map, ownHash + numberByX + 1);
        return result;
    }
}
