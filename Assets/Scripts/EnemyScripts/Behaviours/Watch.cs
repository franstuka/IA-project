using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Watch : BehaviourBase
{
    private float triggerRange;
    private float watchRange;
    [SerializeField] private Transform raycastPos;

    [Range(0,360)]
    public float viewAngle;

    public float viewPercent = 0.75f;
    public float soundRange;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    //public Transform head;

    private void Awake()
    {
        triggerRange = GetComponentInParent<SphereCollider>().radius;
        watchRange = triggerRange * viewPercent;
        //head = transform.GetChild(0).transform;
    }

    public bool FindPlayer(GameObject player)
    {
        /*head.position*/
        /*transform.position*/
        /*transform.forward*/
        /*head.forward*/

        //Debug.Log("DONDE ESTAS??");
        Transform target = player.transform.GetChild(0).transform;

        Vector3 dirToTarget = (target.position - raycastPos.position).normalized;
        float disToTarget = Vector3.Distance(raycastPos.position, target.position);

        //Debug.Log(disToTarget);
        //Debug.Log(player.GetComponent<PlayerCombat>().GetSoundlevel());
        //Debug.Log(player.GetComponent<ThirdPersonCharacter>().m_soundProduced / disToTarget);

            //Vector3 dirToTarget = (target.position - transform.position).normalized;
            //Debug.Log("Range: " + watchRange);
            Debug.DrawRay(raycastPos.position, raycastPos.forward * watchRange, Color.red);

            Vector3 vectorA = Quaternion.AngleAxis(viewAngle / 2, Vector3.up) * (raycastPos.forward * watchRange);
            Vector3 vectorB = Quaternion.AngleAxis(-viewAngle / 2, Vector3.up) * (raycastPos.forward * watchRange);

            Debug.DrawRay(raycastPos.position, vectorA, Color.blue);
            Debug.DrawRay(raycastPos.position, vectorB, Color.blue);
            Debug.DrawLine(raycastPos.position, target.position, Color.green);

            if (Vector3.Angle(raycastPos.forward, dirToTarget) < viewAngle / 2){
                //Debug.Log("dentro del aungulo de vision");
                //float disToTarget = Vector3.Distance(transform.position, target.position);
                if(!Physics.Raycast(raycastPos.position, dirToTarget, disToTarget, obstacleMask))
                {
                    //Debug.Log("TE VEO!");
                    return true;
                }
            }
            else if(player.GetComponent<PlayerCombat>().GetSoundlevel() / disToTarget > 7f)
            {
                    //Debug.Log("TE OIGO!!");
                    return true;
            }

        return false;
        
        //Collider[] targetList = Physics.OverlapSphere(transform.position, watchRange, targetMask);
        //Debug.Log(targetList[0].gameObject.name);
        //Debug.Log(targetList);
    }
} 
