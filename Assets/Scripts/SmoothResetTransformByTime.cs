using UnityEngine;

public class SmoothResetTransformByTime : MonoBehaviour
{
    [Header("Time Settings")]
    [SerializeField] float resetInterval = 120f;
    [SerializeField] float returnDuration = 2f;

    Vector3 startPos;
    Quaternion startRot;

    float timer = 0f;
    float returnTimer = 0f;
    bool isReturning = false;

    Vector3 fromPos;
    Quaternion fromRot;

    void Start()
    {
        startPos = transform.position;
        startRot = transform.rotation;
    }

    void Update()
    {
        if (!isReturning)
        {
            timer += Time.deltaTime;

            if (timer >= resetInterval)
            {
                BeginReturn();
            }
        }
        else
        {
            SmoothReturn();
        }
    }

    void BeginReturn()
    {
        isReturning = true;
        timer = 0f;
        returnTimer = 0f;

        fromPos = transform.position;
        fromRot = transform.rotation;
    }

    void SmoothReturn()
    {
        returnTimer += Time.deltaTime;
        float t = returnTimer / returnDuration;

        transform.position = Vector3.Lerp(fromPos, startPos, t);
        transform.rotation = Quaternion.Slerp(fromRot, startRot, t);

        if (t >= 1f)
        {
            transform.position = startPos;
            transform.rotation = startRot;
            isReturning = false;
        }
    }
}
