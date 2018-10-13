using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alert : BehaviourBase
{
    private RaycastHit[] enemyList;
    [SerializeField] private float alertDistance; //Get y Set

    private float GetalertDistance()
    {
        return alertDistance;
    }

    private void SetalertDistance(float alertDistance)
    {
        this.alertDistance = alertDistance;
    }

    public List<Skeleton> SkeletonInRange(Skeleton other)
    {
        enemyList = Physics.SphereCastAll(other.transform.position, alertDistance, Vector3.one, 11); //11 = enemy layer
        List<Skeleton> skeletonList = new List<Skeleton>();

        for (int i = 0; i < enemyList.Length; i++)
        {
            if(enemyList[i].collider.gameObject.tag == "Enemy")
            {
               if( enemyList[i].collider.gameObject.GetComponent<Skeleton>() != null)
                {
                    skeletonList.Add(enemyList[i].collider.gameObject.GetComponent<Skeleton>());
                }
            }
        }
        return skeletonList;
    }
}
