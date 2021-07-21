using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour
{
    public Rigidbody rb;
    void Start()
    {
        rb.velocity = new Vector3(0, 1, 0);
    }
}
