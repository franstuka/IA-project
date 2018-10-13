using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hold : BehaviourBase
{

    [SerializeField] Transform holdedPosition;
    [SerializeField] Transform faceToThisPosition;


    public Vector3 ReturnToPosition()
    {
        return holdedPosition.position;
    }

    public Vector3 DirectionToFace()
    {
        return faceToThisPosition.position;
    }

}
