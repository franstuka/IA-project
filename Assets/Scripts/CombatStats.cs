using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatStats : MonoBehaviour {

    public enum CombatStatsType{ MAXHP , HP , DAMAGE , DEFENSE, LIGHTVISION };
    [SerializeField] private float MaxHp = 1;
    [SerializeField] private float HP = 1;
    [SerializeField] private float Damage = 0;
    [SerializeField] private float Defense = 0;
    protected RagdollControll ragdollControll;

    public Animator anim;
    
    [SerializeField] protected AudioSource DoDamageSound;
    [SerializeField] private AudioSource GetDamageSound;
    [SerializeField] private AudioSource DieSound;
    [SerializeField] private bool IsAttacking;
    [SerializeField] private bool hasAttackedSomeone;
    [SerializeField] private int attackID;
    [SerializeField] private int enemyLastAttackID;


    private void OnEnable()
    {
        HP = MaxHp;
    }



    private void Awake()
    {
        ragdollControll = GetComponent<RagdollControll>();
        anim = GetComponent<Animator>();
        
        
    }
    // Use this for initialization
    void Start () {
        IsAttacking = false;
    }
	
	// Update is called once per frame
	void Update () {
	}

    public float GetHP()
    {
        return HP;
    }

    public virtual float GetDamage()
    {
        return Damage;
    }

    public float GetDefense()
    {
        return Defense;
    }

    public virtual void ChangeStats(CombatStatsType state , float valor)
    {
        switch((int)state)
        {
            case 0:
                MaxHp += valor;
                if (MaxHp < 0)
                {
                    MaxHp = 0;
                }
                if (MaxHp < HP)
                {
                    HP = MaxHp;
                }
                break;
            case 1:
                if(valor < 0)
                {
                    if(1-Defense > 0)
                        HP += valor * (1 - Defense);
                }
                else
                {
                    HP += valor;
                }    
                if (HP < 0)
                {
                    HP = 0;
                    Die();
                }
                else
                    if(HP>MaxHp)
                        HP = MaxHp;
                break;
            case 2:
                Damage += valor;
                if (Damage < 0)
                    Damage = 0;
                break;
            case 3:
                Defense += valor;
                if (Defense < 0)
                    Defense = 0;
                break;
        }
    }

    public bool GetIsAttacking()
    {
        return IsAttacking;
    }
    public void SetIsAttacking(bool IsAttacking)
    {
        this.IsAttacking = IsAttacking;
    }

    public bool GetHasAttackedSomeone()
    {
        return hasAttackedSomeone;
    }
    public void SetHasAttackedSomeone(bool hasAttackedSomeone)
    {
        this.hasAttackedSomeone = hasAttackedSomeone;
    }

    public int GetAttackID()
    {
        return attackID;
    }
    public void SetAttackID(int attackID)
    {
        this.attackID = attackID;
    }

    public int GetEnemyLastAttackID()
    {
        return enemyLastAttackID;
    }
    public void SetEnemyLastAttackID(int enemyLastAttackID)
    {
        this.enemyLastAttackID = enemyLastAttackID;
    }

    public virtual void Die()
    {

    }
}
