using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorSetter : MonoBehaviour
{

    private Material material;
    void Start()
    {
        Vector4 myColor = Color.HSVToRGB(Random.Range(0f, 1f), 1, 1);
        material = GetComponent<MeshRenderer>().material;
        material.SetVector("_InputColor", myColor);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.frameCount % 200 == 0)
        {
            Vector4 myColor = Color.HSVToRGB(Random.Range(0f, 1f), 1, 1);
            material.SetVector("_InputColor", myColor);
        }
    }
}
