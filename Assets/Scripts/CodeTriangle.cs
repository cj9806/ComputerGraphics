using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodeTriangle : MonoBehaviour
{
    private Mesh customMesh;

    void Start()
    {
        // First, let's create a new mesh
        var mesh = new Mesh();

        // Vertices
        // locations of vertices
        var verts = new Vector3[3];

        verts[0] = new Vector3(0, 0, 0);
        verts[1] = new Vector3(0, 1, 0);
        verts[2] = new Vector3(1, 0, 0);
        mesh.vertices = verts;

        // Indices
        // determines which vertices make up an individual triangle
        //
        // this should always be a multiple of three
        //
        // each triangle should be specified in _clock-wise_ order
        var indices = new int[3];

        indices[0] = 0;
        indices[1] = 1;
        indices[2] = 2;

        mesh.triangles = indices;

        // Normals
        // describes how light bounces off the surface (at the vertex level)
        //
        // note that this data is interpolated across the surface of the triangle
        var norms = new Vector3[3];

        norms[0] = -Vector3.forward;
        norms[1] = -Vector3.forward;
        norms[2] = -Vector3.forward;

        mesh.normals = norms;

        // UVs, STs
        // defines how textures are mapped onto the surface
        var UVs = new Vector2[3];

        UVs[0] = new Vector2(0, 0);
        UVs[1] = new Vector2(0, 1);
        UVs[2] = new Vector2(1, 0);

        mesh.uv = UVs;

        var filter = GetComponent<MeshFilter>();
        filter.mesh = mesh;
        customMesh = mesh;
    }

    void OnDestroy()
    {
        // Clean up our mesh if we created it
        if (customMesh != null)
        {
            Destroy(customMesh);
        }
    }
}
