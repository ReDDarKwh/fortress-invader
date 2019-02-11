using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;
using System;
using System.Linq;

public class BuildingController : MonoBehaviour
{

    public float width;
    public float height;
    public LineRenderer lineRenderer;
    public PolygonCollider2D nav2DCollider;
    public PolygonCollider2D characterCollider;
    public float navColliderOffset = 1;

    private Vector2[] corners;


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

        corners = getCorners(navColliderOffset);

        setupLineRenderer();
        setupCollider(nav2DCollider, corners);
        setupCollider(characterCollider, getCorners(0));
    }

    private void setupLineRenderer()
    {
        lineRenderer.SetPositions(new Vector3[]{
            new Vector3(-width / 2, - height / 2),
            new Vector3( width / 2, - height / 2),
            new Vector3( width / 2,   height / 2),
            new Vector3(-width / 2,   height / 2),
            new Vector3(-width / 2, - height / 2)
        });
    }

    private Vector2[] getCorners(float offset = 0)
    {
        return new Vector2[]{
            new Vector2(-(width / 2 + offset),- (height / 2 + offset)),
            new Vector2(  width / 2 + offset, - (height / 2 + offset)),
            new Vector2(  width / 2 + offset,    height / 2 + offset),
            new Vector2(-(width / 2 + offset),   height / 2 + offset)
        };
    }

    private void setupCollider(PolygonCollider2D collider, Vector2[] corners)
    {

        collider.SetPath(0, new Vector2[]{
            corners[0],
            corners[1],
            corners[2],
            corners[3],
            corners[0],
        });
    }
}
