using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothDimmer : MonoBehaviour
{
    Light light;
    public float intensity;
    float outerAngle;
    // Start is called before the first frame update
    void Start()
    {
        light = this.GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        light.intensity = intensity;
    }
}
