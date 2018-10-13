using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventarySystem : MonoBehaviour
{
    #region Singleton

    public static InventarySystem instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one instance of inventory is trying to active");
            return;
        }

        instance = this;
    }

    #endregion

    public List<Item> inventory = new List<Item>();
    public Weapon weapon;
    private GameObject player;
    private float dropOffsetZ = 0.8f;
    private float dropOffsetY = 1f;
    [SerializeField] private PickUpText text;

    private void Start()
    {
        player = GameObject.Find("Player");
        if (!player)
        {
            Debug.LogError("Can't find player");
        }
        else
        {
            weapon = player.GetComponent<PlayerCombat>().GetWeapon();
        }
    }

    public void AddNewElement(Item item)
    {
        /*if (!item.isDefaultItem)
        {
            inventory.Add(item);
        }*/

        if ( item is Weapon)
        {
            if (weapon != null) 
                DropWeapon();
            weapon = (Weapon)item;
            player.GetComponent<PlayerCombat>().SetWeapon(weapon);
            text.AddTextToQueue(weapon.GetDescription(weapon));
        }
        else
        {
            if (item is Buff)
            {
                Buff buff = (Buff)item;
                player.GetComponent<PlayerCombat>().ChangeStats(buff.stat, buff.BuffValor);
                text.AddTextToQueue(buff.GetDescription(buff));
                //inventory.Add(item);
            }
            else
                inventory.Add(item);
        }
       
    }

    public void DeleteElement(Item item)
    {
        inventory.Remove(item);
    }

    public void DropWeapon()
    {
        GameObject spawnThis = Instantiate(weapon.prefab, player.transform.position, player.transform.rotation);
        spawnThis.transform.position += spawnThis.transform.TransformVector(new Vector3(0, dropOffsetY / spawnThis.transform.lossyScale.y, dropOffsetZ / spawnThis.transform.lossyScale.z));  
    }   //Es necesatio multiplicarlo por la escala del objeto sino no funciona bien

    /*private void Update()
    {
        if(Input.GetButtonDown("Drop"))
        {
            DropWeapon();
        }
    }*/
}