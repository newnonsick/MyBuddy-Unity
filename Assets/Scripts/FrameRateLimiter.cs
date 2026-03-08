using UnityEngine;

public class FrameRateLimiter : MonoBehaviour
{
    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        Debug.Log("Locked Frame rate 60");
    }
}
