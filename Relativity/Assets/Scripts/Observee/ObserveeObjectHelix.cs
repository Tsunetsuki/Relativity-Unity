using System.Collections.Generic;
using UnityEngine;

public class ObserveeObjectHelix: MonoBehaviour
{
    private Observer observer;
    public bool IsCopy { get; set; } = false;
    public GameObject original { get; set; } = null;

    public Vector2 spatial_velocity;
    private Vector3 startingPoint;
    private Vector3 stVelocity;
    public Material lorentzMaterial;

    //Helix parameters
    private float R; // radius of helix
    private float a; // time scaling parameter

    void Start() {

        R = 2f;
        a = Mathf.Sqrt(1 + Mathf.Pow(R, 2));

        observer = GameObject.FindGameObjectWithTag("Observer").GetComponent<Observer>();
        stVelocity = new Vector3(spatial_velocity.x * Observer.LIGHT_SPEED, 1, spatial_velocity.y * Observer.LIGHT_SPEED);
        
        if (IsCopy)
        {
            //throw new System.Exception("Copy unimplemented");
            startingPoint = original.transform.position;
            lorentzMaterial = GetComponent<MeshRenderer>().material;
            lorentzMaterial.SetVector("_IndivColor", Color.HSVToRGB(Random.Range(0f, 1f), 1, 1));
        }
        else
        {
            startingPoint = transform.position;
            SpawnCopy();
        }
    }

    Vector3 h(float t) {
        return new Vector3(-R * Mathf.Cos(t), a * t, R * Mathf.Sin(t));// + startingPoint;
    }

    Vector3 hp(float t) {
        return new Vector3(R * Mathf.Sin(t), a, R * Mathf.Cos(t));
    }

    Vector3 FindRootNewton(Vector3 point_on_plane, Vector3 plane_normal) { // , float-> Vector3 f, flolat -> Vector3 fp
        float epsilon = 0.001f;
        float x = 0;
        float xn = x + 2 * epsilon;

        int iterations = 0;
        List<float> xs = new List<float>();
        while (Mathf.Abs(xn - x) > epsilon)
        {
            x = xn;
            float f = Vector3.Dot(h(x), plane_normal) - Vector3.Dot(point_on_plane, plane_normal);
            float fp = Vector3.Dot(hp(x), plane_normal);
            xn = x - f / fp;

            xs.Add(xn);
            iterations++;
            if (iterations > 1000) 
            {
                Debug.Log("More than 100 iterations in the helix approximation");
                return new Vector3(0, 0, 0);
                //throw new System.Exception("More than 100 iterations in the helix approximation");
            }
        }
        return h(xn);
    }

    private void FixedUpdate() {
        if (!IsCopy)
        {
            transform.position = //new Vector3(observer.transform.position.x, 0, observer.transform.position.z) +
                                    FindRootNewton(observer.transform.position, Vector3.up);
        }
        else
        {
            Vector3 intersection = //new Vector3(observer.transform.position.x, 0, observer.transform.position.z) + 
                                    FindRootNewton(observer.transform.position, observer.equitemporalPlaneNormal);
            Vector3 observerFramePos = GameObject.FindGameObjectWithTag("ObservedFrame").transform.position;
            transform.position = observerFramePos + observer.mat.MultiplyPoint3x4(intersection - observer.transform.position);

            Matrix4x4 originalMatrix = original.transform.localToWorldMatrix;

            //transform.rotation = original.transform.rotation;
            transform.localScale = new Vector3(originalMatrix.m00, originalMatrix.m11, originalMatrix.m22);
            //Debug.Log(originalMatrix);

            SetShaderVariables();
        }
    }

    private void SetShaderVariables() {
        lorentzMaterial.SetVector("_OriginalPos", startingPoint);
        lorentzMaterial.SetVector("_OriginalVel", stVelocity);

        Vector3 observedFramePos = GameObject.FindGameObjectWithTag("ObservedFrame").transform.position;
        lorentzMaterial.SetMatrix("_LorentzMatrix", observer.mat);
        lorentzMaterial.SetMatrix("_LorentzMatrixInverse", observer.mat_inverse);
        lorentzMaterial.SetVector("_ObserverPos", observer.transform.position);
        lorentzMaterial.SetVector("_ObserverVel", observer.equitemporalPlaneNormal);
        lorentzMaterial.SetVector("_ObserverFramePos", observedFramePos);
    }

    private void SpawnCopy() {
        Transform observedFrame = GameObject.FindGameObjectWithTag("ObservedFrame").transform;
        var copy = Instantiate(this, observedFrame.position + transform.position, transform.rotation, observedFrame);
        copy.IsCopy = true;
        copy.original = gameObject;
        copy.GetComponent<MeshRenderer>().material = lorentzMaterial;
    }
}
