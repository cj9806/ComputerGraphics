using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodePolygon : MonoBehaviour
{
    private Mesh customMesh;

    public int numberOfSides;
    // Start is called before the first frame update
    void Start()
    {
        //create mesh
        var mesh = new Mesh();
        //createPentagon(mesh);
        mesh.vertices = GenerateVerticies();
        mesh.triangles = GenerateIndices();
        mesh.normals = GenerateNorms();
        mesh.uv = GenerateUVs();
        var filter = GetComponent<MeshFilter>();
        filter.mesh = mesh;
        customMesh = mesh;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void createPentagon(Mesh mesh)
    {
        //give verticies
        var verts = new Vector3[6]
        {
            //center
            new Vector3(0,0,0),
            //top
            new Vector3(0,1,0),
            //top right
            new Vector3(0.97f,0.3f,0),
            //bottom right
            new Vector3(0.6f,-.825f,0),
            //bottom left
            new Vector3(-0.6f,-0.825f,0),
            //top left
            new Vector3(-0.97f,0.3f,0)
        };
        mesh.vertices = verts;
        //give indecies
        var indices = new int[15]
        {
            //top right triangle
            0,1,2,
            //mid right triangle
            0,2,3,
            //bottom triangle
            0,3,4,
            //mid left triangle
            0,4,5,
            //top left triangle
            0,5,1
        };
        mesh.triangles = indices;
        //give normals
        var norms = new Vector3[6];

        norms[0] = -Vector3.forward;
        norms[1] = -Vector3.forward;
        norms[2] = -Vector3.forward;
        norms[3] = -Vector3.forward;
        norms[4] = -Vector3.forward;
        norms[5] = -Vector3.forward;

        mesh.normals = norms;
        //define UVs
        var UVs = new Vector2[6]
        {
            //center
            new Vector2(0,0),
            //top
            new Vector2(0,1),
            //top right
            new Vector2(0.97f,0.3f),
            //bottom right
            new Vector2(0.6f,-.825f),
            //bottom left
            new Vector2(-0.6f,-0.825f),
            //top left
            new Vector2(-0.97f,0.3f)
        };
        mesh.uv = UVs;
        //apply mesh
        var filter = GetComponent<MeshFilter>();
        filter.mesh = mesh;
        customMesh = mesh;
    }

    Vector3[] GenerateVerticies()
    {
        var verts = new Vector3[numberOfSides+1];
        if (numberOfSides < 3) return new Vector3[0];
        verts[0] = new Vector3(0, 0, 0);
        //decrese of cosine per sides added
        //A = interior angle of triangle
        //x = number of sides
        //360x^-1
        float angle = 360f / numberOfSides;

        for(int i = 1; i < verts.Length; i++)
        {
            //if(i== 1)
            //{
            //    verts[i] = new Vector3(0, 1, 0);
            //    continue;
            //}

            float cos = Mathf.Cos((angle * i) * Mathf.Deg2Rad);
            float sin = Mathf.Sin((angle * i) * Mathf.Deg2Rad);

            verts[i] = new Vector3(sin, cos,0);
        }
        return verts;
    }
    int[] GenerateIndices()
    {
        int[] indicies = new int[numberOfSides * 3];
        //used for incrimentation
        int[] nums = new int[numberOfSides];
        for(int i = 0; i < numberOfSides; i++)
        {
            indicies[i*3] = 0;
            indicies[(i*3) + 1] = i + 1;
            indicies[(i*3) + 2] = i + 2;
        }
        indicies[indicies.Length - 1] = 1;
        return indicies;
    }

    Vector3[] GenerateNorms()
    {
        Vector3[] norms = new Vector3[numberOfSides + 1];
        for(int i = 0; i < norms.Length; i++)
        {
            norms[i] = -Vector3.forward;
        }
        return norms;
    }
    Vector2[] GenerateUVs()
    {
        Vector2[] UVs = new Vector2[numberOfSides + 1];
        Vector3[] temp = GenerateVerticies();
        for(int i = 0; i<temp.Length; i++)
        {
            UVs[i].x = temp[i].x;
            UVs[i].y = temp[i].y;

        }
        return UVs;
    }
}
