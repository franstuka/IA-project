using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : EnemyCombat {

	public enum SkeletonState {ATTACK,CHASE,PATROL,HOLD,PLAYER_LOST,RETURNING_TO_POSITION, DIEDSPINNING, FIRST_SEEKING };
    public enum SkeletonAttacks {SWORD_ATTACK}
    public SkeletonState ActiveState; //only public for debug task
    private AttackList attackList;
    private Patrol patrol;
    private Hold hold;
    private Chase chase;
    private Alert alert;
    private Watch watch;
    private Seek seek;
    private Vector3 target;
    private Vector2 nextAtack; //in pos 0 have the attackType, and in 1 the distance for do the attack
    private bool inAttack = false;
    private float boostTime = 3f;
    private float angularSpeedBase;
    private Coroutine boostCoroutine;
    [SerializeField]private WeaponHitboxDetection weaponHitbox;
    private float destructionTime = 5f;

    private void Awake()
    {
        attackList = GetComponent<AttackList>();
        patrol = GetComponent<Patrol>();
        hold = GetComponent<Hold>();
        chase = GetComponent<Chase>();
        alert = GetComponent<Alert>();
        watch = GetComponent<Watch>();
        seek = GetComponent<Seek>();
    }

    private void Start()
    {
        if(!staticEnemy)
        {
            ActiveState = SkeletonState.PATROL;
            target = patrol.GetNewWaipoint(new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity));
            nav.SetDestination(target);
        }
        else
        {
            ActiveState = SkeletonState.RETURNING_TO_POSITION;
            target = hold.ReturnToPosition();
            nav.SetDestination(target);
        }
        angularSpeedBase = nav.angularSpeed;
        nextAtack = attackList.GetNextAttack();
        destructionTime += Random.value * destructionTime; //randomice the destrucction time;
    }

    public void SkeletonStateMachine()
    {
        switch(ActiveState)
        {
            case SkeletonState.ATTACK:
            {
                    if (!inAttack && anim.GetBool("AttackEnded")) //start attack
                    {
                        inAttack = true;
                        anim.SetBool("Attack", true);
                        if (boostCoroutine != null)
                            StopCoroutine(boostCoroutine);
                        nav.angularSpeed = angularSpeedBase * 0.75f; //we want that the enemy spins to the enemy position, but more slow
                        //this change will reverted at the end of animation
                    }
                    else
                    {
                        if(inAttack && anim.GetBool("AttackEnded")) //attack ended
                        {
                            boostCoroutine = StartCoroutine(BoostAngularSpeed());  //we boost the speed when the enemy finish the atack
                            inAttack = false;
                            SetAttackID(anim.GetInteger("AttackID"));
                            nextAtack = attackList.GetNextAttack();
                            ActiveState = SkeletonState.CHASE;
                            nav.SetDestination(target);  
                        }
                        else //in attack anim
                        {
                            FaceObjective(target);
                            SetIsAttacking(anim.GetBool("CalculateDamage"));
                            if (weaponHitbox.GetHitted() && GetIsAttacking())
                            {
                                if(GetAttackID() != anim.GetInteger("AttackID" )) //if this enemy dont has attack yet
                                {
                                    if (weaponHitbox.GetHittedEnemy().GetComponentInParent<PlayerCombat>() != null)
                                    {
                                        weaponHitbox.GetHittedEnemy().GetComponentInParent<PlayerCombat>().ChangeStats(CombatStatsType.HP, -GetDamage()); //do damage
                                        SetAttackID(anim.GetInteger("AttackID"));
                                        weaponHitbox.GetHittedEnemy().GetComponentInParent<PlayerCombat>().SetEnemyLastAttackID(GetAttackID());
                                    }                             
                                }
                                else
                                {
                                    if(GetAttackID() != weaponHitbox.GetHittedEnemy().GetComponentInParent<PlayerCombat>().GetEnemyLastAttackID())
                                    {
                                        if(weaponHitbox.GetHittedEnemy().GetComponentInParent<PlayerCombat>() != null)
                                        {
                                            weaponHitbox.GetHittedEnemy().GetComponentInParent<PlayerCombat>().ChangeStats(CombatStatsType.HP, -GetDamage());
                                            weaponHitbox.GetHittedEnemy().GetComponentInParent<PlayerCombat>().SetEnemyLastAttackID(GetAttackID());
                                        }
                                        
                                        //if we hit 2 enemies we have this case
                                    }
                                    else
                                    {
                                        //El problema esta es que si golpeamos a más de un enemigo y por casualidad tiene el segundo (o posteriores)
                                        //enemigos que reciben el golpe tienen como ultima id del golpe la id de ataque este enemigo, el sistema se cree
                                        //que este enemigo ya ha sido golpeado. En el juego actualmente este error no puede pasar, pero si por ejemplo
                                        //hacemos que el jugador tenga una mascota o invocacion, el error si ocurrira, para solucionarlo podriamos poner
                                        //una lista en cada combatiente, con la id del ser, y la ultima id del hit, de esta forma arreglariamos el problema
                                        //
                                    }
                                }
                            }
                        }
                    }
                    
                    break;
            }
            case SkeletonState.CHASE:
            {
                    nav.SetDestination(target);
                    break;
            }
            case SkeletonState.HOLD:
            {
                    if (!FaceAndCheckObjective(hold.DirectionToFace(), 2f)) //This will be better if we check once per second instead 1 per frame
                    {
                        ActiveState = SkeletonState.RETURNING_TO_POSITION;
                    }

                    break;
            }
            case SkeletonState.PATROL:
            {
                    if (transform.position.x <= target.x + 0.4f && transform.position.x >= target.x - 0.4f && transform.position.z <= target.z +0.4f && transform.position.z >= target.z - 0.4f)
                    {
                        target = patrol.GetNewWaipoint(target);
                        nav.SetDestination(target);
                    }
                    break;
            }
            case SkeletonState.PLAYER_LOST:
            {
                    if (transform.position.x <= target.x + 0.4f && transform.position.x >= target.x - 0.4f && transform.position.z <= target.z + 0.4f && transform.position.z >= target.z - 0.4f)
                    {
                        if (!chase.GetWaiting())
                        {
                            chase.InLastKnowPosition();
                        }
                        if(chase.GetEndChase())
                        {
                            if(staticEnemy)
                            {
                                target = hold.ReturnToPosition();
                                nav.SetDestination(target);
                                ActiveState = SkeletonState.RETURNING_TO_POSITION;
                            }
                            else
                            {
                                target = patrol.GetClosestWaipoint(transform.position);
                                nav.SetDestination(target);
                                ActiveState = SkeletonState.PATROL;
                            }
                        }
                    }
                    break;
            }
            case SkeletonState.RETURNING_TO_POSITION:
            {
                    if (transform.position.x <= target.x + 0.4f && transform.position.x >= target.x - 0.4f && transform.position.z <= target.z + 0.4f && transform.position.z >= target.z - 0.4f)
                    {
                        if(FaceAndCheckObjective(hold.DirectionToFace(),2f))
                        {
                            ActiveState = SkeletonState.HOLD;
                        }
                        else
                        {
                            FaceObjective(hold.DirectionToFace());
                        }
                    }
                    break;
            }
            case SkeletonState.DIED:
            {
                    break;
            }
        }
    }

    private void Update()
    {
        if (ActiveState != SkeletonState.DIED)
        {
            SkeletonStateMachine();
            UpdateAnimator();
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if(ActiveState != SkeletonState.DIED)
        {
            if (other.gameObject.name == "Player") // we can add inside this:  && other.gameobject.GetComponent<PlayerCombat> != null
            {
                if (watch.FindPlayer(other.gameObject) && ActiveState != SkeletonState.ATTACK) //in vision
                {
                    if (Vector3.Distance(other.transform.position, transform.position) < nextAtack[1])
                    {

                        ActiveState = SkeletonState.ATTACK;
                    }
                    else
                    {
                        if (ActiveState != SkeletonState.CHASE)
                        {                          
                            ActiveState = SkeletonState.CHASE;
                            chase.PlayerFound();
                            Alert(this); 
                        }
                        target = other.transform.position;
                    }

                }
                else //out of vision
                {
                    if (ActiveState == SkeletonState.CHASE)
                    {

                        ActiveState = SkeletonState.PLAYER_LOST;
                        chase.PlayerLost(target);
                        nav.SetDestination(target);
                    }
                    if (ActiveState == SkeletonState.ATTACK)
                    {
                        target = other.transform.position;
                    }
                }
            }
            if (other.gameObject.layer == 11) //Enemy layer
            {

                if (!chase.GetplayerLost() && other.gameObject.GetComponent<Chase>().GetplayerLost())
                {
                    other.gameObject.GetComponent<Skeleton>().target = target;
                    other.gameObject.GetComponent<Skeleton>().ActiveState = SkeletonState.CHASE;

                }
            }
        } 
    }

    private void OnTriggerExit(Collider other)
    {
        if (ActiveState != SkeletonState.DIED)
        {
            if (other.gameObject.tag == "Player")
            {
                if (ActiveState == SkeletonState.CHASE)
                {

                    ActiveState = SkeletonState.PLAYER_LOST;
                    chase.PlayerLost(target);
                    nav.SetDestination(target);
                }
            }
        }
    }

    private void UpdateAnimator()
    {
        if (Vector3.Magnitude(nav.velocity) >= 0.30f)
        {
            anim.SetBool("Running", true);
            anim.SetFloat("MovementSpeed", Vector3.Magnitude(nav.velocity)/2);
        }
        else
            anim.SetBool("Running", false);

    }

    public override void Die()
    {
        if (ActiveState != SkeletonState.DIED)
        {
            StopAllCoroutines();
            base.Die(); 
            ActiveState = SkeletonState.DIED;
            anim.SetTrigger("Die");
            Destroy(nav);
            DropItem();
            StartCoroutine(WaitTime());
        }    
    }

    IEnumerator BoostAngularSpeed()
    {
        nav.angularSpeed = angularSpeedBase * 4;
        yield return new WaitForSeconds(boostTime);
        nav.angularSpeed = angularSpeedBase;

    }

    IEnumerator WaitTime()
    {
        yield return new WaitForSeconds(destructionTime);
        StartCoroutine(DestroySkeleton());
    }

    IEnumerator DestroySkeleton()
    {
        Collider[] colliders = GetComponents<Collider>();
        for(int i = 0; i < colliders.Length; i++ )
        {
            if(!colliders[i].isTrigger)
                Destroy(colliders[i]);
        }
       

        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }
    private void Alert(Skeleton other)
    {
        if (other.ActiveState != SkeletonState.CHASE)
        {
            List<Skeleton> enemyList = alert.SkeletonInRange(other);
            for (int i = 0; i < enemyList.Count; i++)
            {
                enemyList[i].target = other.target;
                enemyList[i].ActiveState = SkeletonState.CHASE;
            }
        }
        
    }
}
