using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatrixSetter : MonoBehaviour
{
    public Material material;
    private float LIGHT_SPEED = 1;

    void Update()
    {
        //Matrix4x4 matrix = Matrix4x4.Scale(Vector3.one * (1.5f + Mathf.Sin(Time.time)) * 2);
        float vel = 0.6f * Mathf.Sin(Time.time);
        Matrix4x4 matrix = Boost(new Vector2(vel, vel));
        material.SetMatrix("_LorentzMatrix", matrix);
        material.SetVector("_ObserverPos", new Vector4(transform.position.x, transform.position.y, transform.position.z, 1));
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
                new Vector4(-gamma * beta_x, gamma, -gamma * beta_y, 0),
                new Vector4((gamma - 1) * beta_x * beta_y / sqr(beta), -gamma * beta_y, 1 + (gamma - 1) * sqr(beta_y) / sqr(beta), 0),
                new Vector4(0, 0, 0, 1)
            );
            return boostMat;
        }
    }

    private float sqr(float f) {
        return Mathf.Pow(f, 2);
    }
}
