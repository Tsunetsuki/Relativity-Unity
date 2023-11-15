using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetToPlayer: MonoBehaviour
{
    private GameObject player;
    public Vector3 offset;

    private void Start() {
        player = GameObject.FindGameObjectWithTag("Observer");
    }
    private void FixedUpdate() {
        transform.position = player.transform.position + offset;
    }
}
