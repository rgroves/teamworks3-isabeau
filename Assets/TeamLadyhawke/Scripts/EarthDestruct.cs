using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthDestruct : StateMachineBehaviour {
    public GameObject earth;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Planet960tris: EarthDestruct: OnStateExit, animator name: " + animator.gameObject.name);
        if (animator.gameObject.name == "Planet960tris")
        {
            // This was called from Panet960tris Animator
            Debug.Log("Planet960tris: EarthDestruct: OnStateExit");
            animator.SetBool("Done", true);
        }
        else if(animator.gameObject.name == "DestructoRay")
        {
            // This was called from DestructoRay Animator
            Debug.Log("DestructoRay: EarthDestruct: OnStateExit");
            earth = GameObject.FindGameObjectWithTag("Earth");
            earth.GetComponent<Animator>().SetTrigger("Kaboom");
        }
    }

    // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}
}
