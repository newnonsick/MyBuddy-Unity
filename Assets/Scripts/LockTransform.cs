using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockTransform : MonoBehaviour
{
    Vector3 startLocalPos;
    Quaternion startLocalRot;

    void Awake()
    {
        startLocalPos = transform.localPosition;
    }

    void LateUpdate()
    {
        transform.localPosition = startLocalPos;
    }
}
