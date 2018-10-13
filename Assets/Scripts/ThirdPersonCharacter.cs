using System.Collections;
using UnityEngine;

public class ThirdPersonCharacter : MonoBehaviour
{
	[SerializeField] float m_MovingTurnSpeed = 360;
	[SerializeField] float m_StationaryTurnSpeed = 180;
	[SerializeField] float m_JumpPower = 12f;
	[Range(1f, 4f)][SerializeField] float m_GravityMultiplier = 2f;
	[SerializeField] float m_AnimSpeedMultiplier = 1f;
	[SerializeField] float m_GroundCheckDistance = 0.1f;

    Rigidbody m_Rigidbody;
	Animator m_Animator;
    [SerializeField] bool m_IsGrounded;
	float m_OrigGroundCheckDistance;
	const float k_Half = 0.5f;
	float m_TurnAmount;
	float m_ForwardAmount;
	Vector3 m_GroundNormal;
	float m_CapsuleHeight;
	Vector3 m_CapsuleCenter;


    public Interactionable target;
    bool onWater = false;
    PlayerCombat playerCombat;
    int waterTriggersEntered = 0;

    bool jump;
    bool interact;
    bool enfundar;
    bool attack;
    bool sacudir;


    void Start()
	{
		m_Animator = GetComponent<Animator>();
		m_Rigidbody = GetComponent<Rigidbody>();

        playerCombat = GetComponent<PlayerCombat>();

		m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
		m_OrigGroundCheckDistance = m_GroundCheckDistance;

	}


	public void Move(Vector3 move)
	{

		// convert the world relative moveInput vector into a local-relative
		// turn amount and forward amount required to head in the desired
		// direction.
		if (move.magnitude > 1f) move.Normalize();
		move = transform.InverseTransformDirection(move);
		CheckGroundStatus();
		move = Vector3.ProjectOnPlane(move, m_GroundNormal);
		m_TurnAmount = Mathf.Atan2(move.x, move.z);
		m_ForwardAmount = move.z;

		ApplyExtraTurnRotation();

        // control and velocity handling is different when grounded and airborne:
        if (m_IsGrounded)
        {
            if (m_Animator.GetBool("InteractNow"))
            {
                InteractNow();
                m_Animator.SetBool("InteractNow", false);
            }
            else
            {
                if (interact)
                {
                    UpdateNearestTarget(); //esto actualiza el target, sino no lo hace
                    if (!(target == null))
                    {
                        interact = IsInTarget();
                    }
                    else
                    {
                        interact = false;
                    }
                }
                if (!m_Animator.GetBool("AttackEnded") || playerCombat.GetInAttack()) //in attack
                {
                    if (!playerCombat.GetInAttack() && !m_Animator.GetBool("AttackEnded")) //start attack
                    {
                        playerCombat.SetInAttack(true);
                        playerCombat.ActivateDemoAttackWeapon();
                    }
                    else
                    {
                        if (playerCombat.GetInAttack() && m_Animator.GetBool("AttackEnded")) //attack ended
                        {
                            playerCombat.SetInAttack(false);
                            playerCombat.ActivateDemoWeapon();
                        }
                        else //in attack anim
                        {
                            if (!m_Animator.GetBool("AttackEnded") && playerCombat.GetIsAttacking() && playerCombat.GetWeaponHitbox().GetHitted()) //this controls the attack phase, doing damage if is necesary
                            {
                                if (playerCombat.GetAttackID() != m_Animator.GetInteger("AttackID")) //if this enemy dont has attack yet
                                {
                                    playerCombat.GetWeaponHitbox().GetHittedEnemy().GetComponentInParent<EnemyCombat>().ChangeStats(CombatStats.CombatStatsType.HP, -playerCombat.GetDamage()); //do damage
                                    playerCombat.SetAttackID(m_Animator.GetInteger("AttackID"));
                                    playerCombat.GetWeaponHitbox().GetHittedEnemy().GetComponentInParent<EnemyCombat>().SetEnemyLastAttackID(playerCombat.GetAttackID());

                                }
                                else
                                {
                                    if (playerCombat.GetAttackID() != playerCombat.GetWeaponHitbox().GetHittedEnemy().GetComponentInParent<EnemyCombat>().GetEnemyLastAttackID())
                                    {
                                        playerCombat.GetWeaponHitbox().GetHittedEnemy().GetComponentInParent<EnemyCombat>().ChangeStats(CombatStats.CombatStatsType.HP, -playerCombat.GetDamage());
                                        playerCombat.GetWeaponHitbox().GetHittedEnemy().GetComponentInParent<EnemyCombat>().SetEnemyLastAttackID(playerCombat.GetAttackID());
                                        //if we hit 2 enemies we have this case
                                    }
                                    else
                                    {
                                        //mismo problema nombrado que en la clase skeleton
                                    }
                                }
                            }
                        }
                    }
                }
                if (!interact)
                {
                    HandleGroundedMovement(jump, m_Animator.GetBool("RunDisabled"));
                }
            }
        }
        else
        {
            HandleAirborneMovement();
        }

		// send input and other state parameters to the animator
		UpdateAnimator(move);
	}
    public void UpdateNonPhysicsStats(bool jump, bool interact , bool enfundar , bool attack , bool sacudir)
    {
        this.jump = jump;
        this.interact = interact;
        this.enfundar = enfundar;
        this.attack = attack;
        this.sacudir = sacudir;

    }

	void UpdateAnimator(Vector3 move)
    {                           
		// update the animator parameters
		//m_Animator.SetFloat("Forward", m_ForwardAmount, 0.1f, Time.deltaTime);
		m_Animator.SetBool("OnGround", m_IsGrounded);
        m_Animator.SetBool("Jumping", jump);
        m_Animator.SetBool("Sacudir", sacudir);
        m_Animator.SetBool("Running", move.z != 0);
        m_Animator.SetBool("GoingLeft", move.x < -0.2);
        m_Animator.SetBool("GoingRight", move.x > 0.2);
        
        m_Animator.SetBool("Attack", attack);
        m_Animator.SetBool("OnWater", onWater);

        if(m_Animator.GetBool("WeaponInHand") && onWater)
        {
            enfundar = true;
        }
        playerCombat.SetIsAttacking(m_Animator.GetBool("CalculateDamage"));

        if (m_Animator.GetBool("AttackEnded"))
        {
            playerCombat.SetAttackID(m_Animator.GetInteger("AttackID"));
        }

        m_Animator.SetFloat("speed", move.z * 3); 

        if (enfundar)
        {
            if (m_Animator.GetBool("WeaponInHand"))
            {
                if(!m_Animator.GetBool("RunDisabled"))
                    m_Animator.SetTrigger("Enfundar"); //Weapon in hand change is done in AC
            }
               
            else
            {
                if (!m_Animator.GetBool("RunDisabled"))
                    m_Animator.SetTrigger("Desenfundar");
            }
                
        }
        else
        {
            if(interact)
            {
                if(!m_Animator.GetBool("WeaponInHand") && !m_Animator.GetBool("RunDisabled") && m_IsGrounded)
                {
                    m_Animator.SetTrigger("Recoger");
                }
            }
        }
       
       
        // the anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
        // which affects the movement speed because of the root motion.
        if (m_IsGrounded && move.magnitude > 0)
		{
			m_Animator.speed = m_AnimSpeedMultiplier;
		}
		else
		{
			// don't use that while airborne
			m_Animator.speed = 1;
		}
	}


	void HandleAirborneMovement()
	{
		// apply extra gravity from multiplier:
		Vector3 extraGravityForce = (Physics.gravity * m_GravityMultiplier) - Physics.gravity;
		m_Rigidbody.AddForce(extraGravityForce);

		m_GroundCheckDistance = m_Rigidbody.velocity.y < 0 ? m_OrigGroundCheckDistance : 0.01f;
	}


	void HandleGroundedMovement(bool jump, bool dontMove)
	{
		// check whether conditions are right to allow a jump:
		if (m_Animator.GetBool("JumpNow")) // if the player has to jump and its the exact timing
		{
			// jump!
			m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, m_JumpPower, m_Rigidbody.velocity.z);
            m_IsGrounded = false;
            m_Animator.applyRootMotion = false;
			m_GroundCheckDistance = 0.1f;
		}
        else
        {
            if(!dontMove)
            {             
                m_Rigidbody.AddForce(transform.forward * m_ForwardAmount * 4 / Time.deltaTime, ForceMode.Acceleration);
            }      
        }
	}



    void ApplyExtraTurnRotation()
	{
		// help the character turn faster (this is in addition to root rotation in the animation)
        
        if(!m_Animator.GetBool("RunDisabled"))
        {
            float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, m_ForwardAmount);
            transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);
        }
		
	}

	void CheckGroundStatus()
	{
		RaycastHit hitInfo;
#if UNITY_EDITOR
		// helper to visualise the ground check ray in the scene view
		Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * m_GroundCheckDistance));
#endif
		// 0.1f is a small offset to start the ray from inside the character
		// it is also good to note that the transform position in the sample assets is at the base of the character
		if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, m_GroundCheckDistance))
		{
			m_GroundNormal = hitInfo.normal;
			m_IsGrounded = true;
			m_Animator.applyRootMotion = true;
		}
		else
		{
			m_IsGrounded = false;
			m_GroundNormal = Vector3.up;
			m_Animator.applyRootMotion = false;
		}
	}

    private bool IsInTarget()
    {

        if (Mathf.Abs(Vector3.Distance(transform.position, target.InteractionTransform.position)) <= target.Radius)
        {
            return true;
        }
        return false;
    }

    public void InteractNow()
    {
        target.Interact(target);
        target = null; //Que es mejor dejarlo a null o dejarlo en missing si recojo algo del suelo?
    }

    private void UpdateNearestTarget()
    {
        if (ItemsNear.instance.Size() == 0)
        {
            target = null;
        }
        else
        {
            if (ItemsNear.instance.Size() == 1)
            {
                target = ItemsNear.instance.GetInteractionable(0);
            }
            else
            {
                target = GetClosestPoints();
            }
        }
    }

    private Interactionable GetClosestPoints()
    {
        float min = Mathf.Infinity;
        int index = 0;
        float aux;

        for (int i = 0; i < ItemsNear.instance.Size(); i++)
        {
            aux = Mathf.Abs(Vector3.Distance(transform.position, ItemsNear.instance.GetInteractionable(i).InteractionTransform.position));
            if (aux < min)
            {
                min = aux;
                index = i;
            }
        }
        return ItemsNear.instance.GetInteractionable(index);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Water")
        {
            if(waterTriggersEntered == 0)
                onWater = true;
            waterTriggersEntered++;
            
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Water")
        {
            waterTriggersEntered--;
            if (waterTriggersEntered == 0)
                onWater = false;
        }
    }

    
}

