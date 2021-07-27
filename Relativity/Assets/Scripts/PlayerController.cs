using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public PlayerInput playerInput;
    private Vector3 dir;
    private bool boostFrozen = false;
    private bool manualBoostEnabled = false;
    //private float controllerDeadzone = 0.1f;
    public float maxBeta;
    private bool paused = false;
    private Vector2 inputToSpatialVelocity(Vector2 inputVec) {
        /*if (inputVec.magnitude < controllerDeadzone)
        {
            inputVec = Vector2.zero;
        }*/
        return maxBeta * Observer.LIGHT_SPEED * inputVec;
    }
    public void OnMovement(InputAction.CallbackContext value) {
        Vector2 newDir = value.ReadValue<Vector2>();
        //dir = new Vector3(newDir.x, 0, newDir.y);
        //transform.position = dir;

        if (!manualBoostEnabled)
        {
            GetComponent<Observer>().BoostVel = inputToSpatialVelocity(value.ReadValue<Vector2>());
        }
    }

    public void OnBoostChange(InputAction.CallbackContext value) {
        if (!boostFrozen && manualBoostEnabled)
        {
            GetComponent<Observer>().BoostVel = inputToSpatialVelocity(value.ReadValue<Vector2>());
        }
    }

    public void OnManualBoostToggle(InputAction.CallbackContext value) {
        if (value.started) manualBoostEnabled = !manualBoostEnabled;
    }

    public void OnReset(InputAction.CallbackContext value) {
        if (value.started) transform.localPosition = Vector3.zero;
    }

    public void OnPause(InputAction.CallbackContext value) {
        if (value.started)
        {
            paused = !paused;

            if (paused)
                Time.timeScale = 0;
            else
                Time.timeScale = 1;
        }
    }
}
