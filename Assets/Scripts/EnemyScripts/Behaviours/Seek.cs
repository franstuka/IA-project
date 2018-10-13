using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seek : BehaviourBase {

    public enum State {  }
    [SerializeField] private Transform[] chaseWaypoints;

    private bool timeToSpin = false;   
    private int index = 0;
    private bool endSeek = true;
    private float waitTime = 3f;
    private bool waiting = false;
    private bool first = true;
    private bool second = true;
    private bool firstSeektWaiting = false;
    private bool secondSeekWaiting = false;


    public void PlayerFound()
    {
        endSeek = false;
        waiting = false;
        first = true;
        second = true;
        firstSeektWaiting = false;
        secondSeekWaiting = false;
        StopAllCoroutines();
    }

    public void InicializeSearch()
    {
        endSeek = false;
        timeToSpin = true;
    }
    public void ChangeEndSeek()
    {
        endSeek = false;
    }

    public void ChangeTimeToSpin()
    {
        timeToSpin = true;
    }

    public void SpinTimer()
    {
        waiting = true;
        StartCoroutine(SpinTime());
    }

    IEnumerator SpinTime()
    {
        yield return new WaitForSeconds(waitTime);        
        waiting = false;
        timeToSpin = false;
    }

    public void FirstSeekTimer()
    {
        first = false;
        firstSeektWaiting = true;
        StartCoroutine(FirstSeekTime());
    }

    IEnumerator FirstSeekTime()
    {
        yield return new WaitForSeconds(waitTime);
        firstSeektWaiting = false;        
    }

    public void SecondSeekTimer()
    {
        second = false;
        secondSeekWaiting = true;
        StartCoroutine(SecondSeekTime());
    }

    IEnumerator SecondSeekTime()
    {
        yield return new WaitForSeconds(waitTime);
        secondSeekWaiting = false;
    }


    // Use this for initialization
    public Vector3 SeekFirstPlace()
    {
        //Coge el primero más cercano
        float min = Mathf.Infinity;
        index = 0;
        float aux;

        for (int i = 0; i < chaseWaypoints.Length; i++)
        {
            aux = Mathf.Abs(Vector3.Distance(transform.position, chaseWaypoints[i].position));
            if (aux < min)
            {
                min = aux;
                index = i;
            }
        }

        return chaseWaypoints[index].position;
    }

    public Vector3 SeekSecondPlace(int index)
    {
        //Coge el segundo más cercano
        float min = Mathf.Infinity;
        int secondIndex = 0;
        float aux;

        for (int i = 0; i < chaseWaypoints.Length; i++)
        {
            if (i != index)
            {
                aux = Mathf.Abs(Vector3.Distance(transform.position, chaseWaypoints[i].position));
                if (aux < min)
                {
                    min = aux;
                    secondIndex = i;
                }
            }

        }
        return chaseWaypoints[secondIndex].position;
    }

    public int GetIndex()
    {
        return index;
    }

    public bool GetendSeek()
    {
        return endSeek;
    }

    public bool GetTimeToSpin()
    {
        return timeToSpin;
    }

    public bool GetWaiting()
    {
        return waiting;
    }

    public bool GetFirstSeekWaiting()
    {
        return firstSeektWaiting;
    }

    public bool GetSecondSeekWaiting()
    {
        return secondSeekWaiting;
    }

    public bool GetFirst()
    {
        return first;
    }

    public bool GetSecond()
    {
        return second;
    }
}
