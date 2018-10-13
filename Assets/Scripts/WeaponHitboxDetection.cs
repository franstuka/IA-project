using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHitboxDetection : MonoBehaviour {

    private bool hit = false;
    private Collider hittedEnemy;

    private void OnTriggerEnter(Collider other)
    {
        if(gameObject.tag == "Player" && other.tag == "Enemy")
        {
            hit = true;
            hittedEnemy = other;
        }
        else
        {
            if (gameObject.tag == "Enemy" && other.tag == "Player")
            {
                hit = true;
                hittedEnemy = other;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (gameObject.tag == "Player" && other.tag == "Enemy")
        {
            hit = false;
        }
        else
        {
            if (gameObject.tag == "Enemy" && other.tag == "Player")
            {
                hit = false;
            }
        }
    }

    public bool GetHitted()
    {
        return hit;
    }
    public Collider GetHittedEnemy()
    {
        return hittedEnemy;
    }
}
