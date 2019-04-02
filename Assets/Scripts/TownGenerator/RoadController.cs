using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using PathCreation;
using PathCreation.Examples;

public class RoadController : MonoBehaviour
{
    public List<Vector3> points;
    public float roadWidth;
    private PathCreator pathcreator;
    private RoadMeshCreator meshCreator;


    // Start is called before the first frame update
    void Start()
    {
        pathcreator = GetComponent<PathCreator>();
        meshCreator = GetComponent<RoadMeshCreator>();

        pathcreator.bezierPath = new BezierPath(points, false, PathSpace.xy);
        pathcreator.bezierPath.ControlPointMode = BezierPath.ControlMode.Automatic;
        pathcreator.bezierPath.AutoControlLength = 0.02f;
        meshCreator.roadWidth = roadWidth;


        meshCreator.CreatePath();

    }

    // Update is called once per frame
    void Update()
    {

    }
}
