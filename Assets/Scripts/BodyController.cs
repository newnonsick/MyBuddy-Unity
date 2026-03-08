using UnityEngine;
using System.Collections;

public class BodyController : MonoBehaviour
{
    public Animator animator;
    public AudioSource audioSource;

    //public int talkingCount = 4;

    bool wasSpeaking;

    void Update()
    {
        bool isSpeaking = audioSource != null && audioSource.isPlaying;

        //if (isSpeaking && !wasSpeaking)
        //{
        //    float randomTalk = Random.Range(0, talkingCount);
        //    Debug.Log("random talk:" + randomTalk);
        //    animator.SetFloat("talkIndex", randomTalk);
        //}

        animator.SetBool("isSpeaking", isSpeaking);
        wasSpeaking = isSpeaking;
    }
}

