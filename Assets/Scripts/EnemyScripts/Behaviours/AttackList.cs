using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackList : BehaviourBase
{
    [SerializeField] private float[] attackProbabilities;
    [SerializeField] private float[] attackRange;

    private void Awake()
    {
        if(attackProbabilities.Length != attackRange.Length)
        {
            Debug.LogError("AttackProbabilities & attackRange not match in size");
        }
    }
    private void Start()
    {
        ScaleAttackProb(); //like in enemySpawn
    }

    private void ScaleAttackProb()
    {
        float acumulatedProb = 0;
        float newAcumulatedProb = 0;
        float scaleFactor = 0;
        for (int i = 0; i < attackProbabilities.Length; i++)
        {
            acumulatedProb += attackProbabilities[i];
        }
        if (acumulatedProb != 100 && acumulatedProb != 0) //rebalance, if acumulatedProb = 0 , the attack dont work
        {
            scaleFactor = 100f / acumulatedProb;
            for (int i = 0; i < attackProbabilities.Length - 1; i++)
            {
                attackProbabilities[i] *= scaleFactor;
                newAcumulatedProb += attackProbabilities[i];
            }
            attackProbabilities[attackProbabilities.Length - 1] = 100f - newAcumulatedProb; //evade numerical errors while adding
        }
    }

    public Vector2 GetNextAttack()
    {
        float valueSum = 0;
        float randomValue = Random.value * 100f;
        int i = 0;
        while ( i < attackProbabilities.Length)
        {
            valueSum += attackProbabilities[i];
            if (valueSum >= randomValue)
            {
                break;
            }
            i++;
        }
        return new Vector2(i, attackRange[i]);
    }
}

