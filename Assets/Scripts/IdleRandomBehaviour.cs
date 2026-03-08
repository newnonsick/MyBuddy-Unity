using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleRandomBehaviour : StateMachineBehaviour
{
    public float unNormalIdleCount = 6;

    override public void OnStateExit(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex
    )
    {
        float randomIdle = Random.Range(1, 34);
        if ( randomIdle == 33 )
        {
            float randomUnNormalIdle = Random.Range(1, 6);
            if ( randomUnNormalIdle != 5 )
            {
                float randomUnNormalIdleIndex = Random.Range(10, 13);
                animator.SetFloat("idleIndex", randomUnNormalIdleIndex);
            } else
            {
                float randomUnNormalIdleIndex = Random.Range(13, 16);
                animator.SetFloat("idleIndex", randomUnNormalIdleIndex);
            }
        } else
        {
            animator.SetFloat("idleIndex", 0);
        }
    }
}
