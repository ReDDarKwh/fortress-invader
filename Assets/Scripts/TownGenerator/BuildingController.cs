using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;
using System;
using System.Linq;
using TownGenerator.Geom;
using UnityEditor;

public class BuildingController : MonoBehaviour
{


    public PolygonCollider2D nav2DCollider;
    public PolygonCollider2D characterCollider;
    public float navColliderOffset = 1;


    public Polygon shape;

    private Vector3[] corners;
    private MeshRenderer meshRenderer;
    private MeshFilter filter;

    public Vector2[] WorldSpaceCorners
    {
        get
        {
            return corners == null ? new Vector2[] { } :
             corners.Select(x => (Vector2)transform.TransformPoint(x)).ToArray();
        }
    }

    // Use this for initialization
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        filter = GetComponent<MeshFilter>();
        Init();
    }

    public void Init()
    {

        //shape = new Polygon(points.Select(x => new Point(x)).ToList());

        var corners = getCorners(navColliderOffset);//.Select(x => new Vector3(x.x, x.y)).ToArray();
        this.corners = corners.Select(x => new Vector3(x.x, x.y)).ToArray();

        //setupLineRenderer();
        setupCollider(nav2DCollider, corners);
        setupCollider(characterCollider, getCorners(0));

        setupMesh();
    }

    private void setupMesh()
    {
        // Create Vector2 vertices
        Vector2[] poly = shape.Select(x => x.vec).ToArray();

        // Use the triangulator to get indices for creating triangles
        // Triangulator triangulator = new Triangulator(poly);
        // int[] tris = triangulator.Triangulate();
        // Mesh m = new Mesh();
        // Vector3[] vertices = new Vector3[poly.Length * 2];

        // for (int i = 0; i < poly.Length; i++)
        // {
        //     vertices[i].x = poly[i].x;
        //     vertices[i].y = poly[i].y;
        //     vertices[i].z = -10; // front vertex
        //     vertices[i + poly.Length].x = poly[i].x;
        //     vertices[i + poly.Length].y = poly[i].y;
        //     vertices[i + poly.Length].z = 0;  // back vertex    
        // }
        // int[] triangles = new int[tris.Length * 2 + poly.Length * 6];
        // int count_tris = 0;
        // for (int i = 0; i < tris.Length; i += 3)
        // {
        //     triangles[i] = tris[i];
        //     triangles[i + 1] = tris[i + 1];
        //     triangles[i + 2] = tris[i + 2];
        // } // front vertices
        // count_tris += tris.Length;
        // for (int i = 0; i < tris.Length; i += 3)
        // {
        //     triangles[count_tris + i] = tris[i + 2] + poly.Length;
        //     triangles[count_tris + i + 1] = tris[i + 1] + poly.Length;
        //     triangles[count_tris + i + 2] = tris[i] + poly.Length;
        // } // back vertices
        // count_tris += tris.Length;
        // for (int i = 0; i < poly.Length; i++)
        // {
        //     // triangles around the perimeter of the object
        //     int n = (i + 1) % poly.Length;
        //     triangles[count_tris] = i;
        //     triangles[count_tris + 1] = n;
        //     triangles[count_tris + 2] = i + poly.Length;
        //     triangles[count_tris + 3] = n;
        //     triangles[count_tris + 4] = n + poly.Length;
        //     triangles[count_tris + 5] = i + poly.Length;
        //     count_tris += 6;
        // }
        // // Create the mesh
        // Mesh msh = new Mesh();
        // msh.vertices = vertices;
        // msh.triangles = triangles;
        // msh.RecalculateNormals();
        // msh.RecalculateBounds();
        // MeshUtility.Optimize(msh);
        // //msh.Optimize();

        // // Set up game object with mesh;
        filter.mesh = Triangulator.CreateMesh(poly, 2);
    }

    // private void setupLineRenderer()
    // {
    //     // copy list, add start to end to finish line path
    //     var lines = new Polygon(shape).Select(v => new Vector3(v.x, v.y)).ToList();

    //     lineRenderer.positionCount = (lines.Count);

    //     lineRenderer.SetPositions(lines.ToArray());
    // }

    private Polygon getCorners(float offset)
    {
        return offset == 0 ? shape : shape.buffer(shape.Select((x) => offset).ToList());
    }

    private void setupCollider(PolygonCollider2D collider, Polygon corners)
    {
        collider.SetPath(0, corners.Concat(new List<Point> { corners[0] }).Select(x => x.vec).ToArray());
    }
}
