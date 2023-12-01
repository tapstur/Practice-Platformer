using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockHeadPath : MonoBehaviour
{
    Transform[] rockHeadPathPoints = new Transform[4];
    void Awake()
    {
        int x = 0;
        foreach(Transform child in GetComponentsInChildren<Transform>())
        {
            if(child != gameObject.transform)
            {
                rockHeadPathPoints[x] = child;
                x++;
            }
        }
    }

    public Transform[] GetPath()
    {
        return rockHeadPathPoints;
    }

}
