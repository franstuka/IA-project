using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Buff", menuName = "Inventory/Buff")]

public class Buff : Item {

    public float BuffValor;
    public CombatStats.CombatStatsType stat;
    public GameObject prefab;

    public string GetDescription(Buff item)
    {
        return item.name + " , add " + item.BuffValor + " " + item.stat + ".";
    }
}
