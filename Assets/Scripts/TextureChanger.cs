using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureChanger : MonoBehaviour
{
    Material material;

    public Texture texture;
    // Start is called before the first frame update
    void Start()
    {
        material = this.GetComponent<Renderer>().material;

        material.SetTexture("_BaseMap", texture);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
