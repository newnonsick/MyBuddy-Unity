using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkingRandomBehaviour : StateMachineBehaviour
{
    public float talkingCount = 4;

    override public void OnStateExit(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex
    )
    {
        float randomTalking = Random.Range(0, talkingCount);
        animator.SetFloat("talkIndex", randomTalking);
    }
}
