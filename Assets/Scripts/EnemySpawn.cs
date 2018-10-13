using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour {

    [SerializeField] private GameObject[] enemieList;
    [SerializeField] private float[] spawnProportion;

    [SerializeField] private EnemySpawnSite[] spawnPlace;


    public Queue<GameObject> spawnQueue;

    [SerializeField] private float spawnProbabilitiePerSecond;

    [SerializeField] private bool addOneAtBeggining = false;
    [SerializeField] private bool oneUseScript = false;
    

    private void Awake()
    {
        if(enemieList.Length != spawnProportion.Length)
        {
            Debug.LogError("EnemieList and spawnProbabilities don´t match in size");
        }
        spawnQueue = new Queue<GameObject>();
        ScaleSpawnProb();
    }

    private void Start()
    {
        if (addOneAtBeggining)
            AddEnemieToQueue();
        if(oneUseScript)
        {
            if (addOneAtBeggining)
                SpawnEnemie();
            else
                Debug.LogWarning("Deleteable spawner dont spaw nothing at beggining");
            Destroy(this);
        }
        else
            StartCoroutine(SpawnLoop());
    }

    private void ScaleSpawnProb()
    {
        float acumulatedProb = 0;
        float newAcumulatedProb = 0;
        float scaleFactor = 0;
        for(int i = 0; i < spawnProportion.Length; i++)
        {
            acumulatedProb += spawnProportion[i];
        }
        if(acumulatedProb != 100 && acumulatedProb != 0) //rebalance, if acumulatedProb = 0 , the spawn dont work
        {     
            scaleFactor = 100f / acumulatedProb;
            for(int i = 0; i < spawnProportion.Length -1; i++)
            {
                spawnProportion[i] *= scaleFactor;
                newAcumulatedProb += spawnProportion[i];
            }
            spawnProportion[spawnProportion.Length - 1] = 100f - newAcumulatedProb; //evade numerical errors while adding
        }
    }

    private void AddEnemieToQueue()
    {
        
        float valueSum = 0;
        float randomValue = Random.value * 100f;
        for (int i = 0; i < spawnProportion.Length; i++)
        {
            valueSum += spawnProportion[i];
            if(valueSum >= randomValue)
            {
                spawnQueue.Enqueue(enemieList[i]);
                return;
            }
        }
    }

    private void SpawnEnemie()
    {
        
        List<int> validValues = new List<int>();
        for(int i = 0; i < spawnPlace.Length; i++)
        {
            if(spawnPlace[i].IsActive())
            {
                validValues.Add(i);
            }
        }
       
        if (validValues.Count >0)
        {
            int randomPos = Mathf.FloorToInt(Random.Range(0f, 0.9999f) * validValues.Count);
            Instantiate(spawnQueue.Dequeue(), spawnPlace[validValues[randomPos]].GetTransform());
        }
    }
    
    IEnumerator SpawnLoop()
    {
        for(; ;)
        {
            if (spawnProbabilitiePerSecond >= Random.Range(0.00001f,100f))
            {
                AddEnemieToQueue();
            }
            if(spawnQueue.Count != 0)
            {
                SpawnEnemie();
            }
            yield return new WaitForSeconds(1f);
        }
    } 
}
