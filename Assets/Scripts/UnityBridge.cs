using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class UnityBridge : MonoBehaviour
{
    [Header("Refs")]
    public AudioSource audioSource;
    public Animator animator;

    bool isSpeaking;


    public void Speak(string audioPath)
    {
        Debug.Log("[UnityBridge] Speak: " + audioPath);
        StopAllCoroutines();
        StartCoroutine(PlayAudio(audioPath));
    }

    public void StopSpeak()
    {
        Debug.Log("[UnityBridge] StopSpeak");

        if (audioSource.isPlaying)
            audioSource.Stop();

        isSpeaking = false;
        animator.SetBool("isSpeaking", false);
    }

    public void PlayAnimation(string animIndexStr)
    {
        if (!int.TryParse(animIndexStr, out int animIndex))
        {
            Debug.LogError("Invalid animIndex: " + animIndexStr);
            return;
        }

        Debug.Log("[UnityBridge] PlayAnimation: " + animIndex);

        animator.SetFloat("animIndex", animIndex);
        animator.SetTrigger("playAnim");
    }


    IEnumerator PlayAudio(string path)
    {
        string url = "file://" + path;

        using (UnityWebRequest req =
            UnityWebRequestMultimedia.GetAudioClip(url, AudioType.WAV))
        {
            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(req.error);
                yield break;
            }

            AudioClip clip = DownloadHandlerAudioClip.GetContent(req);

            audioSource.clip = clip;
            audioSource.Play();

            isSpeaking = true;
            animator.SetBool("isSpeaking", true);
        }
    }

    void Update()
    {
        if (isSpeaking && !audioSource.isPlaying)
        {
            isSpeaking = false;
            animator.SetBool("isSpeaking", false);
        }

#if UNITY_EDITOR
        // test
        if (Input.GetKeyDown(KeyCode.R))
            PlayAnimation("1"); // wave
        if (Input.GetKeyDown(KeyCode.F))
            PlayAnimation("5"); // wave
        if (Input.GetKeyDown(KeyCode.G))
            PlayAnimation("6"); // wave

        if (Input.GetKeyDown(KeyCode.T))
            Speak("C:/Users/New/Downloads/cb749b3ff7b24bf38bcd822b7bfaf8f3941c8811bf8fec53f58349b7.wav");

        if (Input.GetKeyDown(KeyCode.S))
            StopSpeak();
#endif
    }
}
