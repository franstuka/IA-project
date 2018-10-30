using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Watch : BehaviourBase
{
    private float triggerRange;
    private float watchRange;
    private Transform raycastPos;

    //[Range(0,360)]
    //private float viewAngle;

    [SerializeField] private float viewPercent = 0.75f;
    [SerializeField] private float soundRange;

    private LayerMask obstacleMask;

    private void Awake()
    {
        triggerRange = GetComponent<SphereCollider>().radius;
        watchRange = triggerRange * viewPercent;
        obstacleMask = LayerMask.GetMask("Unwalkable");
        raycastPos = transform.Find("CenterView").transform;
    }

    public bool FindPlayer(GameObject player, float viewAngle)
    {

        Transform target = player.transform.Find("CenterView").transform;

        if (target == null)
        {
            Debug.LogWarning("Player's CenterView not detected");
        }

        Vector3 dirToTarget = (target.position - raycastPos.position).normalized;
        float disToTarget = Vector3.Distance(raycastPos.position, target.position);

        Debug.DrawRay(raycastPos.position, raycastPos.forward * watchRange, Color.red);

        Vector3 vectorA = Quaternion.AngleAxis(viewAngle, Vector3.up) * (raycastPos.forward * watchRange);
        Vector3 vectorB = Quaternion.AngleAxis(-viewAngle, Vector3.up) * (raycastPos.forward * watchRange);

        Debug.DrawRay(raycastPos.position, vectorA, Color.blue);
        Debug.DrawRay(raycastPos.position, vectorB, Color.blue);
        Debug.DrawLine(raycastPos.position, target.position, Color.green);

        if (Vector3.Angle(raycastPos.forward, dirToTarget) < viewAngle){

            if(!Physics.Raycast(raycastPos.position, dirToTarget, disToTarget, obstacleMask))
            {
                return true;
            }
        }
        else if(player.GetComponent<PlayerCombat>().GetSoundlevel() / disToTarget > 7f)
        {
            Debug.Log("NO ESTOY SORDO");
                return true;
        }

        return false;
    }
} 
