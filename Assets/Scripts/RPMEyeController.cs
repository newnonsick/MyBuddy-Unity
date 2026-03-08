using UnityEngine;
using System.Collections;

public class RPMEyeBlinkController : MonoBehaviour
{
    [Header("References")]
    public SkinnedMeshRenderer headRenderer;

    [Header("BlendShape Names")]
    public string blinkLeftName = "Blink L";
    public string blinkRightName = "Blinl R";

    [Header("Blink Timing")]
    public float blinkIntervalMin = 2.5f;
    public float blinkIntervalMax = 5.5f;

    [Header("Blink Speed")]
    public float blinkCloseSpeed = 30f;
    public float blinkOpenSpeed = 15f;

    int blinkLIdx;
    int blinkRIdx;

    float blinkValue;
    float blinkTarget;

    void Start()
    {
        blinkLIdx = Get(blinkLeftName);
        blinkRIdx = Get(blinkRightName);

        StartCoroutine(BlinkLoop());
    }

    int Get(string name)
    {
        int idx = headRenderer.sharedMesh.GetBlendShapeIndex(name);
        if (idx < 0)
            Debug.LogWarning($"BlendShape '{name}' not found");
        return idx;
    }

    void LateUpdate()
    {
        float speed = blinkTarget > blinkValue
            ? blinkCloseSpeed
            : blinkOpenSpeed;

        blinkValue = Mathf.Lerp(
            blinkValue,
            blinkTarget,
            Time.deltaTime * speed
        );

        Set(blinkLIdx, blinkValue);
        Set(blinkRIdx, blinkValue);
    }

    IEnumerator BlinkLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(
                Random.Range(blinkIntervalMin, blinkIntervalMax)
            );

            yield return BlinkOnce();

            // ~20% chance double blink (very human)
            if (Random.value < 0.2f)
            {
                yield return new WaitForSeconds(0.08f);
                yield return BlinkOnce();
            }
        }
    }

    IEnumerator BlinkOnce()
    {
        blinkTarget = 100f; // close
        yield return new WaitForSeconds(0.07f);

        blinkTarget = 0f;   // open
        yield return new WaitForSeconds(0.12f);
    }

    void Set(int idx, float v)
    {
        if (idx >= 0)
            headRenderer.SetBlendShapeWeight(idx, v);
    }
}
