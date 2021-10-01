using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChanger : MonoBehaviour
{
    Material material;

    public Color color;
    // Start is called before the first frame update
    void Start()
    {
        material = this.GetComponent<Renderer>().material;
        material.SetColor("_BaseColor", color);
    }
}
