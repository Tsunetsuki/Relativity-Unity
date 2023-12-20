using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Observer : MonoBehaviour
{
    private Rigidbody rb;
    public Matrix4x4 mat;
    public Matrix4x4 mat_inverse;

    public LineRenderer tBasisVector;
    public LineRenderer xBasisVector;
    public LineRenderer yBasisVector;
    public Material lorentzMaterial;

    [HideInInspector]
    public Vector3 equitemporalPlaneNormal;
    public Vector2 BoostVel { get; set; }
    public bool BoostIsSetManually { get; set; }
    private Vector2 spatial_acc = Vector2.zero;

    public static float LIGHT_SPEED = 1;

    public AudioSource audioSource = null;

    private void Awake() {
        mat = Matrix4x4.identity;
        mat_inverse = Matrix4x4.Inverse(mat);
        equitemporalPlaneNormal = Vector3.up;
        BoostVel = Vector2.zero;
        rb = GetComponent<Rigidbody>();

        audioSource = GameObject.FindGameObjectWithTag("Radio")?.GetComponent<AudioSource>();
    }

    private void Update() {
        tBasisVector.SetPosition(1, mat_inverse.MultiplyPoint3x4(new Vector3(0, 1, 0)));
        xBasisVector.SetPosition(1, mat_inverse.MultiplyPoint3x4(new Vector3(1, 0, 0)));
        yBasisVector.SetPosition(1, mat_inverse.MultiplyPoint3x4(new Vector3(0, 0, 1)));

        equitemporalPlaneNormal = Vector3.Cross(mat_inverse.MultiplyPoint3x4(new Vector3(1, 0, 0)), mat_inverse.MultiplyPoint3x4(new Vector3(0, 0, 1))).normalized;
        Debug.DrawLine(transform.position, equitemporalPlaneNormal);
        
    }

    private void FixedUpdate() {
        mat = BoostIsSetManually ? Boost(BoostVel) : Boost(spatial_acc) * mat;
        mat_inverse = Matrix4x4.Inverse(mat);

        Vector3 stVelocity = mat_inverse.MultiplyPoint3x4(new Vector3(0, 1, 0));
        
        if (!BoostIsSetManually)
            rb.velocity = stVelocity;

        if (audioSource)
            audioSource.pitch = 1 + Mathf.Log10(stVelocity.y);
        SetShaderVariables();
    }

    public void Accelerate(Vector2 spatial_acc) {
        this.spatial_acc = spatial_acc;
        //Vector3 newVel = Boost(spatial_acc).inverse.MultiplyPoint3x4(Vector3.up);
        //st_acc = mat.MultiplyPoint3x4(newVel);//in stationary frame
    }

    private void SetShaderVariables() {
        Vector3 observedFramePos = GameObject.FindGameObjectWithTag("ObservedFrame").transform.position;
        lorentzMaterial.SetMatrix("_LorentzMatrix", mat);
        lorentzMaterial.SetMatrix("_LorentzMatrixInverse", mat_inverse);
        lorentzMaterial.SetVector("_ObserverPos", transform.position);
        lorentzMaterial.SetVector("_ObserverVel", equitemporalPlaneNormal);
        lorentzMaterial.SetVector("_ObserverFramePos", observedFramePos);

    }

    private Matrix4x4 Boost(Vector2 twoVel) {

        float spatialSpeed = twoVel.magnitude; // spatialSpeed < light_speed
        if (spatialSpeed == 0)
        {
            return Matrix4x4.identity;
        }
        {
            float beta = spatialSpeed / LIGHT_SPEED;
            float beta_x = twoVel.x / LIGHT_SPEED;
            float beta_y = twoVel.y / LIGHT_SPEED;
            float gamma = 1 / Mathf.Sqrt(1 - sqr(beta));

            Matrix4x4 boostMat = new Matrix4x4(
                new Vector4(1 + (gamma - 1) * sqr(beta_x) / sqr(beta), -gamma * beta_x, (gamma - 1) * beta_x * beta_y / sqr(beta), 0),
                new Vector4(-gamma * beta_x, gamma, - gamma * beta_y, 0),
                new Vector4((gamma - 1) * beta_x * beta_y / sqr(beta) , - gamma * beta_y, 1 + (gamma - 1) * sqr(beta_y) / sqr(beta), 0),
                new Vector4(0, 0, 0, 1)
            );
            return boostMat;
        }
    }

    /*private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + st_acc);
        Gizmos.color = Color.white;
    }*/

    private float sqr(float f) {
        return Mathf.Pow(f, 2);
    }
    private float Cosh(float f) { return (float)System.Math.Cosh(f); }
    private float Sinh(float f) { return (float)System.Math.Sinh(f); }
    private float Tanh(float f) { return (float)System.Math.Tanh(f); }
    private float ArcTanh(float f) { return (float) (0.5 * System.Math.Log((1 + f) / (1 - f))); }
}
