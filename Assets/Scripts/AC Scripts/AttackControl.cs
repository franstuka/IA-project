using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackControl : StateMachineBehaviour {

    [SerializeField] bool finalAnimation = false;
    float actualTime = 0;
    float maxTimeToContinue = 0;
    float minTimeToContinue = 0;
    float maxTimeToCalculateDamage;
    float minTimeToCalculateDamage;
    [SerializeField] float maxTimeToContinueFactor = 0.90f;
    [SerializeField] float minTimeToContinueFactor = 0.25f;
    [SerializeField] float maxTimeToCalculateDamageFactor = 0.7f;
    [SerializeField] float minTimeToCalculateDamageFactor = 0.25f;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        animator.SetBool("Attack", false);
        animator.SetInteger("AttackID", animator.GetInteger("AttackID") +1);
        actualTime = 0;
        maxTimeToContinue = stateInfo.length * maxTimeToContinueFactor;//if the player click attack between this animations time, he will continue the combo
        minTimeToContinue = stateInfo.length * minTimeToContinueFactor;
        maxTimeToCalculateDamage = stateInfo.length * maxTimeToCalculateDamageFactor;
        minTimeToCalculateDamage = stateInfo.length * minTimeToCalculateDamageFactor;
        animator.SetBool("RunDisabled", true);
        animator.SetBool("AttackEnded", false);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        actualTime += Time.deltaTime;
        if (animator.GetBool("Attack") && actualTime >= minTimeToContinue && actualTime <= maxTimeToContinue)
        {
            animator.SetBool("ContinueAttack", true);
        }

        if (actualTime >= minTimeToCalculateDamage && actualTime <= maxTimeToCalculateDamage)
        {
            animator.SetBool("CalculateDamage", true);
        }
        else
        {
            animator.SetBool("CalculateDamage", false);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if(!animator.GetBool("ContinueAttack") || finalAnimation)
        {
            animator.SetBool("RunDisabled", false);
            animator.SetBool("AttackEnded", true);
        }
        animator.SetBool("ContinueAttack", false);
        
    }

    // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}
}
