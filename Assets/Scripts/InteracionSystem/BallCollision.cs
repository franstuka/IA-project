using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallCollision : MonoBehaviour {

    public static bool ballCollided = false;

    // Use this for initialization
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Scenario")
        {
            ballCollided = true;
        }
    }
    

}

