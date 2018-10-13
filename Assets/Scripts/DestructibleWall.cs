using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleWall : MonoBehaviour {

    [SerializeField] private GameObject prefabRoto;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && other.gameObject.GetComponentInParent<PlayerCombat>().GetWeaponType() == Weapon.WeaponType.MACE && other.gameObject.GetComponentInParent<PlayerCombat>().GetIsAttacking())
        {

            prefabRoto.SetActive(true);
            this.gameObject.SetActive(false);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player" && other.gameObject.GetComponentInParent<PlayerCombat>().GetWeaponType() == Weapon.WeaponType.MACE && other.gameObject.GetComponentInParent<PlayerCombat>().GetIsAttacking())
        {

            prefabRoto.SetActive(true);
            this.gameObject.SetActive(false);
        }
    }
    
}
