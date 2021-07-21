using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public PlayerInput playerInput;
    private Vector3 dir;
    private float rapidity;
    public void OnMovement(InputAction.CallbackContext value) {
        Vector2 newDir = value.ReadValue<Vector2>();
        dir = new Vector3(newDir.x, newDir.y, 0);
        transform.position = dir;
    }

    public void OnBoostChange(InputAction.CallbackContext value) {
        //Vector2 inputDir = value.ReadValue<Vector2>();
        GetComponent<Observer>().BoostVel = 0.7f * Observer.LIGHT_SPEED * value.ReadValue<Vector2>();
    }
}
