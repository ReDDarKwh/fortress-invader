using System.Collections;
using System.Collections.Generic;
using TownGenerator.Geom;
using UnityEngine;
using System.Linq;

public class WardController : MonoBehaviour
{

    public PolygonCollider2D collider2D;

    [System.NonSerialized]
    public Polygon shape;

    // Start is called before the first frame update
    void Start()
    {
        setupCollider(collider2D, shape);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void setupCollider(PolygonCollider2D collider, Polygon corners)
    {
        collider.SetPath(0, corners.Concat(new List<Point> { corners[0] }).Select(x => x.vec).ToArray());
    }
}
