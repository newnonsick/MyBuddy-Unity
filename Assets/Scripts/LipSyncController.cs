using UnityEngine;
using System.Collections.Generic;

public class RPMLipSyncController : MonoBehaviour
{
    [Header("References")]
    public AudioSource audioSource;
    public SkinnedMeshRenderer headRenderer;

    [Header("Audio")]
    public int sampleSize = 512;
    public float sensitivity = 3.2f;
    public float noiseGate = 0.008f;

    [Header("Smoothing")]
    public float attackSpeed = 28f;
    public float releaseSpeed = 10f;

    [Header("Jaw / Mouth Power")]
    public float jawBoost = 1.6f;
    public float vowelMax = 85f;
    public float consonantMax = 55f;

    // BlendShapes
    int A, EI, O, U;
    int BMP, FV, TH, CH;

    float[] samples;
    float intensity;
    float peak; // short-term memory

    Dictionary<int, float> current = new Dictionary<int, float>();

    void Start()
    {
        samples = new float[sampleSize];

        A = Get("A");
        EI = Get("E;I");
        O = Get("O");
        U = Get("U");

        BMP = Get("B,M,P");
        FV = Get("F,V");
        TH = Get("TH");
        CH = Get("CH,j,SH");
    }

    int Get(string name)
    {
        int idx = headRenderer.sharedMesh.GetBlendShapeIndex(name);
        if (idx >= 0) current[idx] = 0f;
        else Debug.LogWarning($"Missing BlendShape: {name}");
        return idx;
    }

    void LateUpdate()
    {
        float raw = 0f;

        if (audioSource && audioSource.isPlaying)
        {
            audioSource.GetOutputData(samples, 0);

            for (int i = 0; i < samples.Length; i++)
                raw += samples[i] * samples[i];

            raw = Mathf.Sqrt(raw / samples.Length);

            if (raw > noiseGate)
                raw = Mathf.Clamp01(raw * sensitivity);
            else
                raw = 0f;
        }

        // =========================
        // Peak Hold (improves timing)
        // =========================
        peak = Mathf.Max(raw, peak - Time.deltaTime * 2.5f);
        intensity = Mathf.Clamp01((raw + peak) * 0.5f);

        // Non-linear response (key!)
        intensity = Mathf.Pow(intensity, 0.65f);

        float speed = intensity > 0.02f ? attackSpeed : releaseSpeed;

        // =========================
        // VOWELS (mouth open)
        // =========================

        float jaw = intensity * jawBoost;

        Apply(A, jaw * vowelMax, speed);
        Apply(EI, intensity * (1f - intensity) * vowelMax * 0.7f, speed);
        Apply(O, intensity * intensity * vowelMax, speed);
        Apply(U, intensity * 0.55f * vowelMax, speed);

        // =========================
        // CONSONANTS (shape detail)
        // =========================

        float c = intensity * consonantMax;

        Apply(BMP, c * Pulse(6f), speed);
        Apply(FV, c * Pulse(4f), speed);
        Apply(TH, c * Pulse(3f), speed);
        Apply(CH, c * Pulse(5f), speed);
    }

    float Pulse(float speed)
    {
        return Mathf.Clamp01(Mathf.PerlinNoise(Time.time * speed, 0f));
    }

    void Apply(int idx, float target, float speed)
    {
        if (idx < 0) return;

        float cur = current[idx];
        cur = Mathf.Lerp(cur, target, Time.deltaTime * speed);
        current[idx] = cur;

        headRenderer.SetBlendShapeWeight(idx, cur);
    }
}
