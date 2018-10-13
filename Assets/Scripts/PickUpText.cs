using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpText : MonoBehaviour {

    Queue<string> colaTextos;
    [SerializeField] TMPro.TextMeshProUGUI[] textBoxes;
    [SerializeField] bool[] textBoxInUse;
    [SerializeField] Transform[] initialPos;
    [SerializeField] Transform[] midPos;
    [SerializeField] Transform[] endPos;
    [SerializeField] float initTime;
    [SerializeField] float midTime;
    [SerializeField] float endTime;

    int lastUsed;

    bool courutineStarted = false;

    private void Awake()
    {
        //colaTextos = new Queue<string>();
        textBoxInUse = new bool[textBoxes.Length]; //supongo que inician en false
    }

    private void Start()
    {
        colaTextos = new Queue<string>();
    }

    public void AddTextToQueue(string text)
    {
        colaTextos.Enqueue(text);
        if(!courutineStarted)
        {
            courutineStarted = true;
            StartCoroutine(ShowTextBucle());
        }
    }

    private void ShowText(int i)
    {
        
        textBoxes[i].text = colaTextos.Dequeue();
    }

    IEnumerator ShowTextBucle()
    {
        while(colaTextos.Count > 0)
        {
            for(int i = 0; i < textBoxInUse.Length;i++)
            {
                if(!textBoxInUse[i])
                {
                    //textBoxes[i].gameObject.SetActive(true);
                    textBoxes[i].transform.position = initialPos[i].position;
                    textBoxInUse[i] = true;
                    ShowText(i);
                    lastUsed = i;
                    StartCoroutine(Move());
                    break;
                }
            }
            yield return new WaitForSeconds(0.25f);
        }
        courutineStarted = false;
    }

    IEnumerator Move()
    {
        int j = lastUsed;
        float time = 0;
        while(time <= initTime)
        {
            time += Time.deltaTime;
            textBoxes[j].transform.position += (midPos[j].position - initialPos[j].position) * Time.deltaTime *2 ;
            yield return new WaitForFixedUpdate();
        }
        yield return new WaitForSeconds(midTime);
        time = 0;
        while (time <= endTime)
        {
            time += Time.deltaTime;
            textBoxes[j].transform.position += (endPos[j].position - midPos[j].position) * Time.deltaTime * 2;
            yield return new WaitForFixedUpdate();
        }
        textBoxInUse[j] = false;
        //textBoxes[i].gameObject.SetActive(false);
    }
}
