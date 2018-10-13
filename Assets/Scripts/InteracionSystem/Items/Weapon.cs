using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Inventory/Weapon")]
public class Weapon : Item
{
    public enum WeaponType { SWORD , RAPIER , MACE}
    public float Damage;
    public WeaponType weapon;
    public GameObject prefab;

    public string GetDescription(Weapon item)
    {
        return item.name + " , type " + item.weapon + " , " + item.Damage + " damage.";
    }
}
