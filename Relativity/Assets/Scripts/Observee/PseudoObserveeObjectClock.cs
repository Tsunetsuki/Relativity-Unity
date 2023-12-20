using UnityEngine;

public class PseudoObserveeObjectClock: MonoBehaviour
{
    private Observer observer;
    public bool IsCopy { get; set; } = false;
    public GameObject original { get; set; } = null;

    public Vector2 spatial_velocity;
    private Vector3 startingPoint;
    private Vector3 stVelocity;
    public Material lorentzMaterial;

    
    public float R;

    void Start() {
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
    Vector2 getClockPosition(float t) {
        return new Vector2(-R * Mathf.Cos(t), R * Mathf.Sin(t));
    }

    private void FixedUpdate() {
        float observerTime = observer.transform.position.y;
        Vector2 clockPosition = getClockPosition(observerTime);

        if (!IsCopy)
        {
            transform.position = observer.transform.position +
                                    new Vector3(clockPosition.x, 0, clockPosition.y);
        }
        else
        {
            Vector3 observerFramePos = GameObject.FindGameObjectWithTag("ObservedFrame").transform.position;
            transform.position = observerFramePos +
                    new Vector3(clockPosition.x, 0, clockPosition.y);

            Matrix4x4 originalMatrix = original.transform.localToWorldMatrix;
            transform.localScale = new Vector3(originalMatrix.m00, originalMatrix.m11, originalMatrix.m22);
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
