using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathFade : MonoBehaviour
{
    Material material;

    float alpha;

   
    public float fadeSpeed = 1.0f; //the Higher the value the slower the fadeout will be
    // Start is called before the first frame update
    void Start()
    {
        material = this.GetComponent<Renderer>().material;

        Color test = new Color(1.0f, 1.0f, 1.0f, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        Color fadeOut = new Color(material.color.r, material.color.g, material.color.b, (material.color.a - .04f / fadeSpeed));
        material.SetColor("_BaseColor", fadeOut);
    }


}
