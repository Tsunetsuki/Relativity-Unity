using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public PlayerInput playerInput;
    private bool manualBoostEnabled = false;

    public float maxBeta;
    public float maxAcc;
    private bool paused = false;
    private Vector2 spatial_acc;
    private Vector2 inputToSpatialVelocity(Vector2 inputVec) {
        return maxBeta * Observer.LIGHT_SPEED * inputVec;
    }

    public void OnMovement(InputAction.CallbackContext value) {
        if (!paused)
        {
            Vector2 inputVec = value.ReadValue<Vector2>();
            spatial_acc = maxAcc * Observer.LIGHT_SPEED * inputVec;
        }
    }

    private void FixedUpdate() {
        if (!manualBoostEnabled)
        {
            GetComponent<Observer>().Accelerate(spatial_acc);
        }
    }

    public void OnBoostChange(InputAction.CallbackContext value) {
        if (manualBoostEnabled && !paused)
        {
            GetComponent<Observer>().BoostVel = inputToSpatialVelocity(value.ReadValue<Vector2>());
        }
    }

    public void OnManualBoostToggle(InputAction.CallbackContext value) {
        if (value.started) manualBoostEnabled = !manualBoostEnabled;
        GetComponent<Observer>().BoostIsSetManually = manualBoostEnabled;
    }

    public void OnReset(InputAction.CallbackContext value) {
        if (value.started)
        {
            transform.localPosition = Vector3.zero;
            GetComponent<Observer>().mat = Matrix4x4.identity;
        }
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
