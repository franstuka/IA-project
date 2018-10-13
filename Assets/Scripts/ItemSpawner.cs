using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour { //This class is soo similar to enemy spawn, but in a simpliest way

    [SerializeField] private GameObject[] ObjectsList;
    [SerializeField] private float[] spawnProportion;

    [SerializeField] private Transform spawnPlace;
    [SerializeField] private float spawnProbability;



    private void Awake()
    {
        if (ObjectsList.Length != spawnProportion.Length)
        {
            Debug.LogError("ObjectsList and spawnProportion don´t match in size");
        }
        ScaleSpawnProb();
    }

    private void Start()
    {
        SelectAndSpawnObject();
        Destroy(this);
    }

    private void ScaleSpawnProb()
    {
        float acumulatedProb = 0;
        float newAcumulatedProb = 0;
        float scaleFactor = 0;
        for (int i = 0; i < spawnProportion.Length; i++)
        {
            acumulatedProb += spawnProportion[i];
        }
        if (acumulatedProb != 100 && acumulatedProb != 0) //rebalance, if acumulatedProb = 0 , the spawn dont work
        {
            scaleFactor = 100f / acumulatedProb;
            for (int i = 0; i < spawnProportion.Length - 1; i++)
            {
                spawnProportion[i] *= scaleFactor;
                newAcumulatedProb += spawnProportion[i];
            }
            spawnProportion[spawnProportion.Length - 1] = 100f - newAcumulatedProb; //evade numerical errors while adding
        }
    }

    private void SelectAndSpawnObject()
    {

        float valueSum = 0;
        float randomSpawnProbability = Random.value * 100f;
        if (randomSpawnProbability <= spawnProbability )
        {
            float randomValue = Random.value * 100f;
            for (int i = 0; i < spawnProportion.Length; i++)
            {
                valueSum += spawnProportion[i];
                if (valueSum >= randomValue)
                {
                    Instantiate(ObjectsList[i], spawnPlace);
                    return;
                }
            }
        }
    } 
}
