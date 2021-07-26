using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera camera0;
    public Camera camera1;
    public Camera camera2;

    public void SetActiveCamera(int camera) {
        switch (camera)
        {
            case 0:
                camera0.gameObject.SetActive(true);
                camera1.gameObject.SetActive(false);
                camera2.gameObject.SetActive(false);
                break;
            case 1:
                camera0.gameObject.SetActive(false);
                camera1.gameObject.SetActive(true);
                camera2.gameObject.SetActive(false);
                break;
            case 2:
                camera0.gameObject.SetActive(false);
                camera1.gameObject.SetActive(false);
                camera2.gameObject.SetActive(true);
                break;
        }
    }
}
