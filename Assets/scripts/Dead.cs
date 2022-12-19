using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dead : StateMachineBehaviour
{

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0,0);
        animator.gameObject.GetComponent<BoxCollider2D>().enabled = false;
        Destroy(animator.gameObject);
    }

}
