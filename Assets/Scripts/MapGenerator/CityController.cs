using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Delaunay;
using UnityEngine;

public class CityController : MonoBehaviour
{


    public int size;

    public int seed;

    public int relaxIterations;

    public int numberOfPatches;

    public int snapDistance;

    public MapGraph mapGraph;

    public HeightMapSettings heightMapSettings;

    public PointGeneration pointGeneration;
    public int pointSpacing = 10;


    [System.NonSerialized]
    public List<MapGraph.MapNode> heightSortedNodes = new List<MapGraph.MapNode>();

    public enum PointGeneration
    {
        Random,
        PoissonDisc,
        OffsetGrid,
        Grid
    }


    // Start is called before the first frame update
    void Start()
    {
        Init();
    }






    public void Init()
    {

        var startTime = DateTime.Now;

        heightSortedNodes = new List<MapGraph.MapNode>();

        var time = DateTime.Now;
        var voronoi = new Voronoi(
                    GetPoints(),
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


        HashSet<MapGraph.MapNode> visited = new HashSet<MapGraph.MapNode>();


        TakeHighest(mapGraph.nodesByCenterPosition.Values.OrderByDescending(x => x.GetElevation()).First(), visited);
    }

    private void TakeHighest(MapGraph.MapNode mapNode, HashSet<MapGraph.MapNode> visited)
    {
        heightSortedNodes.Add(mapNode);
        visited.Add(mapNode);

        var n = mapNode.GetNeighborNodes().Where(x => !visited.Contains(x)).OrderByDescending(x => x.GetElevation()).FirstOrDefault();

        if (n != null && heightSortedNodes.Count < numberOfPatches)
        {
            TakeHighest(n, visited);
        }
    }

    // Update is called once per frame
    void Update()
    {

        foreach (var node in heightSortedNodes)
        {

            foreach (var edge in node.GetEdges())
            {

                Debug.DrawLine(edge.GetStartPosition(), edge.GetEndPosition(), Color.green);

            }

        }
    }

    private List<Vector2> GetPoints()
    {
        List<Vector2> points = null;
        if (pointGeneration == PointGeneration.Random)
        {
            points = VoronoiGenerator.GetVector2Points(seed, (size / pointSpacing) * (size / pointSpacing), size);
        }
        else if (pointGeneration == PointGeneration.PoissonDisc)
        {
            var poisson = new PoissonDiscSampler(size, size, pointSpacing, seed);
            points = poisson.Samples().ToList();
        }
        else if (pointGeneration == PointGeneration.Grid)
        {
            points = new List<Vector2>();
            for (int x = pointSpacing; x < size; x += pointSpacing)
            {
                for (int y = pointSpacing; y < size; y += pointSpacing)
                {
                    points.Add(new Vector2(x, y));
                }
            }
        }
        else if (pointGeneration == PointGeneration.OffsetGrid)
        {
            points = new List<Vector2>();
            for (int x = pointSpacing; x < size; x += pointSpacing)
            {
                bool even = false;
                for (int y = pointSpacing; y < size; y += pointSpacing)
                {
                    var newX = even ? x : x - (pointSpacing / 2f);
                    points.Add(new Vector2(newX, y));
                    even = !even;
                }
            }
        }

        return points;
    }
}
