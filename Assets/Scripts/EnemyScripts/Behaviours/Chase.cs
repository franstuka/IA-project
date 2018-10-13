using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chase : BehaviourBase
{
    private Vector3 lastKnowPosition;
    private float waitTime = 3f;
    private bool playerLost = true;
    private bool endChase = true;
    private bool waiting = true;

    public void PlayerLost(Vector3 lastKnowPosition)
    {
        playerLost = true;
        this.lastKnowPosition = lastKnowPosition;
        
    }

    public bool GetplayerLost()
    {
        return playerLost;
    }

    public void PlayerFound()
    {
        playerLost = false;
        endChase = false;
        waiting = false;
        StopAllCoroutines();
    }

    public void InLastKnowPosition()
    {
        waiting = true;
        StartCoroutine(LostWaitTime());
    }

    IEnumerator LostWaitTime()
    {
        yield return new WaitForSeconds(waitTime);
        endChase = true;
    }

    public bool GetEndChase()
    {
        return endChase;
    }

    public bool GetWaiting()
    {
        return waiting;
    }

}
