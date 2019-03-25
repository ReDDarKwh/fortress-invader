using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;
using System;
using System.Linq;
using TownGenerator.Geom;

public class BuildingController : MonoBehaviour
{

    public LineRenderer lineRenderer;
    public PolygonCollider2D nav2DCollider;
    public PolygonCollider2D characterCollider;
    public float navColliderOffset = 1;


    public Polygon shape;

    private Vector3[] corners;

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
        Init();
    }

    public void Init()
    {

        //shape = new Polygon(points.Select(x => new Point(x)).ToList());

        var corners = getCorners(navColliderOffset);//.Select(x => new Vector3(x.x, x.y)).ToArray();
        this.corners = corners.Select(x => new Vector3(x.x, x.y)).ToArray();

        setupLineRenderer();
        setupCollider(nav2DCollider, corners);
        setupCollider(characterCollider, getCorners(0));
    }

    private void setupLineRenderer()
    {
        // copy list, add start to end to finish line path
        var lines = new Polygon(shape).Select(v => new Vector3(v.x, v.y)).ToList();

        lineRenderer.positionCount = (lines.Count);

        lineRenderer.SetPositions(lines.ToArray());
    }

    private Polygon getCorners(float offset)
    {
        return offset == 0 ? shape : shape.buffer(shape.Select((x) => offset).ToList());
    }

    private void setupCollider(PolygonCollider2D collider, Polygon corners)
    {
        collider.SetPath(0, corners.Concat(new List<Point> { corners[0] }).Select(x => x.vec).ToArray());
    }
}
