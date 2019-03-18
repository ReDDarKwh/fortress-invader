using System;
using System.Collections;
using System.Collections.Generic;
using Delaunay;
using UnityEngine;

public class CityController : MonoBehaviour
{


    public int size;

    public int seed;

    public int relaxIterations;

    public int numberOfPatches;

    public int snapDistance;

    public int voronoiPointNumber;
    public Voronoi voronoi;
    public MapGraph mapGraph;

    public HeightMapSettings heightMapSettings;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    public void Init()
    {

        var startTime = DateTime.Now;

        var time = DateTime.Now;
        var voronoi = new Voronoi(
                    VoronoiGenerator.GetVector2Points(seed, voronoiPointNumber, size),
                    null, new Rect(0, 0, size, size),
                    relaxIterations
                );
        Debug.Log(string.Format("Voronoi Generated: {0:n0}ms", DateTime.Now.Subtract(time).TotalMilliseconds));

        time = DateTime.Now;
        heightMapSettings.noiseSettings.seed = seed;
        var heightMap = HeightMapGenerator.GenerateHeightMap(size, size, heightMapSettings, Vector2.zero);
        Debug.Log(string.Format("Heightmap Generated: {0:n0}ms", DateTime.Now.Subtract(time).TotalMilliseconds));

        time = DateTime.Now;
        mapGraph = new MapGraph(voronoi, heightMap, snapDistance);
        Debug.Log(string.Format("Finished Generating Map Graph: {0:n0}ms with {1} nodes", DateTime.Now.Subtract(startTime).TotalMilliseconds, mapGraph.nodesByCenterPosition.Count));


    }

    // Update is called once per frame
    void Update()
    {

        foreach (var node in mapGraph.edges)
        {

            Debug.DrawLine(node.GetStartPosition(), node.GetEndPosition(), Color.red);
        }
    }
}
