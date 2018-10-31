using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chase : BehaviourBase
{
    private Vector3 lastKnowPosition;
    private float waitTime = 3f;
    public bool playerLost = true;
    public bool endChase = true;
    public bool waiting = true;
    public bool hasPlayerInSight = false;
    public bool otherHasPlayerInSight = false;

    public void PlayerLost(Vector3 lastKnowPosition)
    {
        playerLost = true;
        hasPlayerInSight = false;
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
        hasPlayerInSight = true;
        StopAllCoroutines();
    }

    public void PlayerByOtherFound()
    {
        playerLost = false;
        endChase = false;
        waiting = false;
        hasPlayerInSight = false;
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

    public bool GetPlayerInSight()
    {
        return hasPlayerInSight;
    }

    public bool GetOtherPlayerInSight()
    {
        return otherHasPlayerInSight;
    }
    public void SetOtherPlayerInSightFalse()
    {
        otherHasPlayerInSight = false;
    }

}
