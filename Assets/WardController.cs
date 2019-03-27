using System.Collections;
using System.Collections.Generic;
using TownGenerator.Geom;
using UnityEngine;
using System.Linq;
using TownGenerator.Wards;



public class WardController : MonoBehaviour
{

    public PolygonCollider2D collider;
    public Ward ward;


    private MeshRenderer meshRenderer;
    private MeshFilter filter;



    // Start is called before the first frame update
    void Start()
    {

        meshRenderer = GetComponent<MeshRenderer>();
        filter = GetComponent<MeshFilter>();
        setupCollider(collider, ward.patch.shape);
        setupMesh();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void setupMesh()
    {
        // Create Vector2 vertices
        Vector2[] poly = ward.patch.shape.Select(x => x.vec).ToArray();
        Triangulator tr = new Triangulator(poly);
        int[] indices = tr.Triangulate();

        // Create the Vector3 vertices
        Vector3[] vertices = new Vector3[poly.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = new Vector3(poly[i].x, poly[i].y, 0);
        }

        // Create the mesh
        Mesh msh = new Mesh();
        msh.vertices = vertices;
        msh.triangles = indices;
        msh.RecalculateNormals();
        msh.RecalculateBounds();

        // Set up game object with mesh;
        filter.mesh = msh;
    }

    private void setupCollider(PolygonCollider2D collider, Polygon corners)
    {
        collider.SetPath(0, corners.Concat(new List<Point> { corners[0] }).Select(x => x.vec).ToArray());
    }
}
