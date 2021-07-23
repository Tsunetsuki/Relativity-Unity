using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObserveeObject : MonoBehaviour
{
    private Observer observer;
    public bool IsCopy { get; set; } = false;
    public GameObject original { get; set; } = null;

    private Vector2 spatial_velocity;
    private Vector3 startingPoint;
    private Vector3 stVelocity;

    void Start() {
        observer = GameObject.FindGameObjectWithTag("Observer").GetComponent<Observer>();
        startingPoint = transform.position;
        spatial_velocity = new Vector2(0.3f, 0.3f) * Observer.LIGHT_SPEED;
        stVelocity = new Vector3(spatial_velocity.x, 1, spatial_velocity.y);
        
        if (!IsCopy)
        {
            SpawnCopy();
        }
    }

    private void FixedUpdate() {
        if (!IsCopy)
        {
            Debug.DrawRay(transform.position, stVelocity);
            transform.position = LinePlaneIntersect(startingPoint, stVelocity);
        }
        else
        {
            Vector3 observerFramePos = GameObject.FindGameObjectWithTag("ObservedFrame").transform.position;
            transform.position = observerFramePos + observer.mat.MultiplyPoint3x4(original.transform.position - observer.transform.position);
        }
    }

    private void SpawnCopy() {
        Transform observedFrame = GameObject.FindGameObjectWithTag("ObservedFrame").transform;
        ObserveeObject copy = Instantiate(this, observedFrame.position + transform.position, transform.rotation, observedFrame);
        copy.IsCopy = true;
        copy.original = gameObject;
    }

    private Vector3 LinePlaneIntersect(Vector3 point_on_line, Vector3 line_direction) {
        Vector3 point_on_plane = observer.transform.position;
        float a = Vector3.Dot(point_on_plane - point_on_line, observer.equitemporalPlaneNormal);
        float b = Vector3.Dot(line_direction, observer.equitemporalPlaneNormal);

        if (b == 0) {
            if (a == 0) Debug.LogWarning("Plane and line are collinear!\n");
            else Debug.LogWarning("Plane and line are parallel!");

            return Vector3.zero;
        }
        else
        {
            float x = a / b;
            return x * line_direction + point_on_line;
        }
    }
}
