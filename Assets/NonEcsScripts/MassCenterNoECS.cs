using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MassCenterNoECS : MonoBehaviour
{
    public bool updated = false;
    public Vector3 center;
    void Update()
    {
        updated = false;
    }

    public Vector3 GetCenter()
    {
        if (!updated)
        {
            float count = 0;
            center = Vector3.zero;
            foreach (var bird in GameObject.FindGameObjectsWithTag("Bird"))
            {
                center += bird.transform.position;
                count++;
            }

            center /= count;
        }

        return center;
    }
}
