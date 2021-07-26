using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObserveeObject : MonoBehaviour
{
    private Observer observer;
    public bool IsCopy { get; set; } = false;
    public GameObject original { get; set; } = null;

    public Vector2 spatial_velocity;
    private Vector3 startingPoint;
    private Vector3 stVelocity;
    public Material lorentzMaterial;

    void Start() {
        observer = GameObject.FindGameObjectWithTag("Observer").GetComponent<Observer>();
        stVelocity = new Vector3(spatial_velocity.x * Observer.LIGHT_SPEED, 1, spatial_velocity.y * Observer.LIGHT_SPEED);
        
        if (IsCopy)
        {
            startingPoint = original.transform.position;
        }
        else
        {
            startingPoint = transform.position;
            SpawnCopy();
        }
    }

    private void FixedUpdate() {
        if (!IsCopy)
        {
            Debug.DrawRay(transform.position, stVelocity);
            transform.position = LinePlaneIntersect(startingPoint, stVelocity, observer.transform.position, Vector3.up);
        }
        else
        {
            Vector3 intersection = LinePlaneIntersect(startingPoint, stVelocity, observer.transform.position, observer.equitemporalPlaneNormal);
            Vector3 observerFramePos = GameObject.FindGameObjectWithTag("ObservedFrame").transform.position;
            transform.position = observerFramePos + observer.mat.MultiplyPoint3x4(intersection - observer.transform.position);
        }
    }

    private void SpawnCopy() {
        Transform observedFrame = GameObject.FindGameObjectWithTag("ObservedFrame").transform;
        ObserveeObject copy = Instantiate(this, observedFrame.position + transform.position, transform.rotation, observedFrame);
        copy.IsCopy = true;
        copy.original = gameObject;
        copy.GetComponent<MeshRenderer>().material = lorentzMaterial;
    }

    private Vector3 LinePlaneIntersect(Vector3 point_on_line, Vector3 line_direction, Vector3 point_on_plane, Vector3 plane_normal) {
        float a = Vector3.Dot(point_on_plane - point_on_line, plane_normal);
        float b = Vector3.Dot(line_direction, plane_normal);

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
