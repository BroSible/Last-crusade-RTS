using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class UnitFollowState : StateMachineBehaviour
{
    AttackController attackController;
    NavMeshAgent agent;
    public float attackingDistance;
    public float rotationSpeed = 5.0f;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        attackController = animator.transform.GetComponent<AttackController>();
        agent = animator.transform.GetComponent<NavMeshAgent>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Should unit transition to idle state?
        if(attackController.targetToAttack == null)
        {
            animator.SetBool("isFollow", false);
        }

        else
        {
            // If there is no other direct command to move
            if(animator.transform.GetComponent<UnitMovement>().isCommandedToMove == false)
            {
                // Moving unit towards enemy
                agent.SetDestination(attackController.targetToAttack.position);

                // Restrict rotation to Y-axis only
                Vector3 direction = attackController.targetToAttack.position - animator.transform.position;
                direction.y = 0; // Ignore Y-axis for rotation

                if (direction != Vector3.zero) // Prevent rotation issues when direction is zero
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    animator.transform.rotation = Quaternion.Slerp(animator.transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
                }
            
                // Should unit transition to attack state?
                float distanceFromTarget = Vector3.Distance(attackController.targetToAttack.position, animator.transform.position);
                if(distanceFromTarget <= attackingDistance)
                {
                    agent.SetDestination(animator.transform.position);
                    animator.SetBool("isAttacking", true);
                    Debug.Log("Can attack");
                }
            }
        }
    }
}
