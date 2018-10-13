using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol : BehaviourBase
{
    [SerializeField] private Transform[] waipoints;
	
    public Vector3 GetNewWaipoint(Vector3 lastWaipoint)
    {
        int newWaipoint = 0;
        bool correct = false;
        while(!correct)
        {
            newWaipoint = Mathf.FloorToInt(Random.Range(0f, 0.999f) * waipoints.Length);
            if (lastWaipoint != waipoints[newWaipoint].position)
                correct = true;
        }
        return waipoints[newWaipoint].position;
    }

    public Vector3 GetClosestWaipoint(Vector3 enemyPos)
    {
        float min = Mathf.Infinity;
        int index = 0;
        float aux;

        for (int i = 0; i < waipoints.Length; i++)
        {
            aux = Mathf.Abs(Vector3.Distance(transform.position, waipoints[i].position));
            if (aux < min)
            {
                min = aux;
                index = i;
            }
        }
        return waipoints[index].position;
    }
}
