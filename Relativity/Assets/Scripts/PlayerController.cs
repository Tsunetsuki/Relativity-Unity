using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public PlayerInput playerInput;
    private Vector3 dir;
    private bool boostFrozen = false;

    public void OnMovement(InputAction.CallbackContext value) {
        Vector2 newDir = value.ReadValue<Vector2>();
        dir = new Vector3(newDir.x, newDir.y, 0);
        transform.position = dir;
    }

    public void OnBoostChange(InputAction.CallbackContext value) {
        if (!boostFrozen)
        {
            Vector2 inputVec = value.ReadValue<Vector2>();
            if (inputVec.magnitude < 0.1)
            {
                inputVec = Vector2.zero;
            }
            GetComponent<Observer>().BoostVel =  0.9f * Observer.LIGHT_SPEED * inputVec;
        }
    }

    public void OnBoostFreeze(InputAction.CallbackContext value) {
        if (value.started)
        {
            boostFrozen = !boostFrozen;
        }
    }
}
