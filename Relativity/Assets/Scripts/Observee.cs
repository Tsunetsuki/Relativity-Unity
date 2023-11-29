using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Observee : MonoBehaviour
{
    private Observer observer;
    public bool IsCopy { get; set; } = false;
    public GameObject original { get; set; } = null;

    void Start() {
        observer = GameObject.FindGameObjectWithTag("Observer").GetComponent<Observer>();

        if (!IsCopy)
        {
            SpawnCopy();
        }
    }

    private void Update() {
        //if (IsCopy)
        //{
        //    Vector3 observerFramePos = GameObject.FindGameObjectWithTag("ObservedFrame").transform.position;
        //    transform.position = observerFramePos + observer.mat.MultiplyPoint3x4(original.transform.position - observer.transform.position);
        //}
    }

    private void SpawnCopy() {
        Transform observedFrame = GameObject.FindGameObjectWithTag("ObservedFrame").transform;
        Observee copy = Instantiate(this, observedFrame.position + transform.position, transform.rotation, observedFrame);
        copy.IsCopy = true;
        copy.original = gameObject;
    }
}
