using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaySphere : MonoBehaviour
{
    [Header("Options")]
    public Color renderColor;
    public float shininess;
    public float specIntensity;

    private void Awake()
    {
        GameObject.FindObjectOfType<CameraTracer>().RegisterSphere(this);
    }

    private void Update()
    {
        GameObject.FindObjectOfType<CameraTracer>().RegisterSphere(this);
    }

}

//Details
public struct SphereData
{

    public Vector3 pos;
    public Vector3 col;

    public Vector3 details;

}