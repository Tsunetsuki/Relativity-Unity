﻿using System.Collections;
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
    public Vector2 BoostVel{ get; set; }

    public static float LIGHT_SPEED = 1;

    private void Awake() {
        mat = Matrix4x4.identity;
        mat_inverse = Matrix4x4.Inverse(mat);
        equitemporalPlaneNormal = Vector3.up;
        BoostVel = Vector2.zero;
        rb = GetComponent<Rigidbody>();
    }

    private void Update() {
        //rapidity = 1 * Mathf.Sin(Time.time);
        mat = Boost(BoostVel);
        mat_inverse = Matrix4x4.Inverse(mat);

        tBasisVector.SetPosition(1, mat_inverse.MultiplyPoint3x4(new Vector3(0, 1, 0)));
        xBasisVector.SetPosition(1, mat_inverse.MultiplyPoint3x4(new Vector3(1, 0, 0)));
        yBasisVector.SetPosition(1, mat_inverse.MultiplyPoint3x4(new Vector3(0, 0, 1)));

        equitemporalPlaneNormal = Vector3.Cross(mat_inverse.MultiplyPoint3x4(new Vector3(1, 0, 0)), mat_inverse.MultiplyPoint3x4(new Vector3(0, 0, 1))).normalized;
        Debug.DrawLine(transform.position, equitemporalPlaneNormal);
        
    }

    private void FixedUpdate() {
        Vector3 stVelocity = mat_inverse.MultiplyPoint3x4(new Vector3(0, 1, 0));
        rb.velocity = stVelocity;
        SetShaderVariables();
    }

    private void SetShaderVariables() {
        Vector3 observedFramePos = GameObject.FindGameObjectWithTag("ObservedFrame").transform.position;
        lorentzMaterial.SetMatrix("_LorentzMatrix", mat);
        lorentzMaterial.SetMatrix("_LorentzMatrixInverse", mat_inverse);
        lorentzMaterial.SetVector("_ObserverPos", new Vector4(transform.position.x, transform.position.y, transform.position.z, 1));
        lorentzMaterial.SetVector("_ObserverFramePos", new Vector4(observedFramePos.x, observedFramePos.y, observedFramePos.z, 1));
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

    private float sqr(float f) {
        return Mathf.Pow(f, 2);
    }
    private float Cosh(float f) { return (float)System.Math.Cosh(f); }
    private float Sinh(float f) { return (float)System.Math.Sinh(f); }
    private float Tanh(float f) { return (float)System.Math.Tanh(f); }
    private float ArcTanh(float f) { return (float) (0.5 * System.Math.Log((1 + f) / (1 - f))); }
}
