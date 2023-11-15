using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetToFrame : MonoBehaviour
{
    public Transform frame;
    public Vector3 offset;
    private void FixedUpdate() {
        transform.position = frame.position + offset;
    }
}
