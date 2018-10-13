using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : CombatStats
{
    [SerializeField] private Weapon Weapon;
    [SerializeField] private WeaponHitboxDetection weaponHitbox;
   
    [SerializeField] private GameObject[] weaponMesh;
    [SerializeField] private Light[] playerLights;
    [SerializeField] private float soundProduded = 25f;

    private bool inAttack = false;

    void Start() {
        SetWeaponOnAnimator();
        UpdateWeaponHitbox();
    }


    public Weapon GetWeapon()
    {
        return Weapon;
    }
    public void SetWeapon(Weapon NewWeapon)
    {
        Weapon = NewWeapon;
        SetWeaponOnAnimator();
        UpdateWeaponHitbox();
    }
    public override float GetDamage()
    {
        return base.GetDamage() + Weapon.Damage;
    }
    public Weapon.WeaponType GetWeaponType()
    {
        return Weapon.weapon;
    }
    private void SetWeaponOnAnimator()
    {
        DesactivateWeaponMesh();
        switch (Weapon.weapon)//weapontype
        {
            case Weapon.WeaponType.SWORD:
                {
                    anim.SetInteger("WeaponType", 1);
                    weaponMesh[0].SetActive(true);
                    break;
                }
            case Weapon.WeaponType.MACE:
                {
                    anim.SetInteger("WeaponType", 2);
                    weaponMesh[1].SetActive(true);
                    break;
                }
            case Weapon.WeaponType.RAPIER:
                {
                    anim.SetInteger("WeaponType", 3);
                    weaponMesh[2].SetActive(true);
                    break;
                }
            default:
                {
                    anim.SetInteger("WeaponType", 0);
                    break;
                }
        }
    }
    public override void ChangeStats(CombatStatsType state, float valor)
    {
        base.ChangeStats(state, valor);
        if(state == CombatStatsType.LIGHTVISION)
        {
            for(int i = 0; i < playerLights.Length; i++)
            {
                playerLights[i].range += valor /2;
                if(playerLights[i].type == LightType.Spot)
                {
                    playerLights[i].spotAngle += valor *2;
                }

            }
        }
    }

    public override void Die()
    {
        base.Die();
        GameManager.instance.EndLevelLost();
        ragdollControll.ActivateRagdoll();
    }

    public void UpdateWeaponHitbox()
    {
        //accederiamos al prefab Guardado para el arma y obtendriamos la componente weaponHitBox
        //No esta implementado porque solo tenemos un combo de ataque, y ahora no hace falta cambiarla.
    }

    public bool GetInAttack()
    {
        return inAttack;
    }

    public void SetInAttack(bool inAttack)
    {
        this.inAttack = inAttack;
    }

    public WeaponHitboxDetection GetWeaponHitbox()
    {
        return weaponHitbox;
    }

    public void DesactivateWeaponMesh()
    {
        for(int i = 0; i < weaponMesh.Length; i++)
        {
            weaponMesh[i].SetActive(false);
        }
    }

    public void ActivateDemoAttackWeapon()
    {
        DesactivateWeaponMesh();
        weaponMesh[0].SetActive(true);
    }
    public void ActivateDemoWeapon()
    {
        DesactivateWeaponMesh();
        weaponMesh[anim.GetInteger("WeaponType") -1].SetActive(true);
    }

    public float GetSoundlevel()
    {
        return soundProduded;
    }

    public void SetSoundlevel(float level)
    {
        soundProduded = level;
    }

}
