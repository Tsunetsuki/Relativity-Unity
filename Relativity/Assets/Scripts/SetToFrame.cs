using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetToFrame : MonoBehaviour
{
    public Transform frame;
    void Start()
    {
        transform.position = frame.position + new Vector3(0, 100, 0);
    }
}
