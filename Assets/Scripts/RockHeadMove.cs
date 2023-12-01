using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockHeadMove : MonoBehaviour
{
    [SerializeField] RockHeadPath myPath;
    public Transform[] myPathPoints;

    Rigidbody2D myRB;

    private void Awake()
    {
        myPathPoints = myPath.GetPath();

    }


    private void Start()
    {
        myRB = GetComponent<Rigidbody2D>();

        transform.position = myPathPoints[0].position;
    }

    public void MoveToNextPoint()
    {
        
    }
}
