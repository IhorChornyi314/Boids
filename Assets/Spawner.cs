using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    public int number;
    public GameObject birdPrefab;
    public float boundX, boundY;
    
    void Start()
    {
        Entity newEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(birdPrefab,
            GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null));
        
        for (int i = 0; i < number; i++)
        {
            Entity newBird = World.DefaultGameObjectInjectionWorld.EntityManager.Instantiate(newEntity);
            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponent(newEntity, typeof(DirectionComponent));
            World.DefaultGameObjectInjectionWorld.EntityManager.SetComponentData(newBird, new Translation
            {
                Value = new float3((Random.value - 0.5f) * boundX, (Random.value - 0.5f) * boundY, 0)
            });
        }
        World.DefaultGameObjectInjectionWorld.EntityManager.DestroyEntity(newEntity);
    }
}
