using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour
{
    public Rigidbody rb;
    public Vector3 velocity;
    void Start()
    {
        rb.velocity = velocity;
    }
}
