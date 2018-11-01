using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seek : BehaviourBase{

    [SerializeField] private Transform[] chaseWaypoints;
    [SerializeField] private Transform[] seekWaypoints;
    private int firstindex;
    private int first = 1;
    private bool timeToSpin = true;
    private bool spinning = false;

    // Use this for initialization
    void Start () {
		
	}

    public Vector3 SeekPlace()
    {
        if (first == 1)
        {
            return SeekFirstPlace();
        }
        else 
        {
            return SeekSecondPlace(firstindex);
        }

    }

    public Vector3 SeekFirstPlace()
    {
        //Coge el primero más cercano
        float min = Mathf.Infinity;
        float aux;
        Vector3 firstPlace = new Vector3();

        for (int i = 0; i < chaseWaypoints.Length; i++)
        {
            aux = Mathf.Abs(Vector3.Distance(transform.position, chaseWaypoints[i].position));
            if (aux < min)
            {
                min = aux;
                firstindex = i;
                firstPlace = chaseWaypoints[i].position;
            }
        }
        first = 2;

        
        return firstPlace;
    }

    public Vector3 SeekSecondPlace(int index)
    {
        //Coge el segundo más cercano
        float min = Mathf.Infinity;
        int secondIndex = 0;
        float aux;
        Vector3 secondPlace = new Vector3();

        for (int i = 0; i < chaseWaypoints.Length; i++)
        {
            if (i != index)
            {
                aux = Mathf.Abs(Vector3.Distance(transform.position, chaseWaypoints[i].position));
                if (aux < min)
                {
                    min = aux;
                    secondIndex = i;
                    secondPlace = chaseWaypoints[i].position;
                }
            }

        }

        first = 3;

        return secondPlace;
    }

    public Vector3 GetSeekPoints()
    {
        if( seekWaypoints.Length != 0)
        {
            int valor = Mathf.RoundToInt(Random.value * (seekWaypoints.Length -1));
            return seekWaypoints[valor].position;
        }

        else
        {
            Debug.LogError("Waypoints Length = 0");
            return new Vector3(0, 0, 0);
        }


    }
    
    public bool GetTimeSpin()
    {
        return timeToSpin;
    }

    public bool GetSpinning()
    {
        return spinning;
    }

    public int GetFirst()
    {
        return first;
    }
    public void SetTimeToSpin(bool active)
    {
        timeToSpin = active;
    }

    public void Setspinning(bool active)
    {
        spinning = active;
    }

    public void SetFirst()
    {
        first = 1;
    }
}
