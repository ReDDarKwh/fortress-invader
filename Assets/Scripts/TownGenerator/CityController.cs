using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RPGKit.FantasyNameGenerator;
using Scripts.AI;
using TownGenerator.Building;
using TownGenerator.Geom;
using TownGenerator.Wards;
using UnityEngine;
using Random = UnityEngine.Random;

public class CityController : MonoBehaviour
{

    public CitySettings settings;
    public bool debugDraw = false;

    [System.NonSerialized]
    public Model cityModel;
    public GameObject buildingPrefab;
    public GameObject wallPrefab;
    public GameObject gatePrefab;
    public GameObject towerPrefab;
    public GameObject wardPrefab;
    public GameObject roadPrefab;
    //public GameObject navGridPrefab;

    public Nav2D navGrid;
    public float gateOpeningSize = 1;

    public string cityName;

    private IEnumerator coroutine;
    private Vector2 spawnPoint;

    void Start()
    {

        Init();
    }

    private IEnumerator GenerateNav2d()
    {
        yield return 0;

        navGrid.transform.position = transform.position - new Vector3(cityModel.cityRadius * transform.localScale.x,
        cityModel.cityRadius * transform.localScale.y);

        navGrid.width = (int)Mathf.Ceil(cityModel.cityRadius * transform.localScale.x * 2 / navGrid.grid.cellSize.x);
        navGrid.height = (int)Mathf.Ceil(cityModel.cityRadius * transform.localScale.y * 2 / navGrid.grid.cellSize.y);

        navGrid.GenerateNavMesh(transform.position, cityModel.cityRadius * transform.localScale.x);
    }

    private void generate()
    {

    }

    private void MovePlayerToSpawnPoint(Vector3 vec)
    {
        GameObject.FindGameObjectWithTag("Player").transform.position = vec;
    }

    public void Init()
    {

        for (var i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        settings.seed = Random.Range(1, 1000000);

        //create the city data model

        cityName = FantasyNameGenerator.GetTownName();

        cityModel = new Model(settings);

        //create buildings
        foreach (var patch in cityModel.patches)
        {

            var ward = Instantiate(wardPrefab, this.transform, false);
            ward.GetComponent<WardController>().ward = patch.ward;

            foreach (var shape in patch.ward.geometry)
            {
                if (shape._perimeter() > 0)
                {
                    var b = Instantiate(buildingPrefab, this.transform, false);
                    b.transform.rotation = Quaternion.identity;

                    Debug.DrawLine(transform.position, cityModel.center.vec, Color.magenta, 20);

                    var buildingController = b.GetComponent<BuildingController>();
                    buildingController.shape = shape;
                }
            }
        }

        // create walls and towers and gates

        if (cityModel.wall != null)
        {

            // city wall and towers
            for (var i = 0; i < cityModel.wall.shape.Count; i++)
            {
                // wall

                var start = cityModel.wall.shape[i].vec;
                var end = cityModel.wall.shape[(i + 1) % cityModel.wall.shape.Count].vec;
                var nextEnd = cityModel.wall.shape[(i + 2) % cityModel.wall.shape.Count].vec;

                var startToEnd = end - start;
                var endToStart = start - end;
                var dis = endToStart.magnitude;

                //connectedToGate

                if (cityModel.wall.gates.Concat(
                    cityModel.citadel != null ? (cityModel.citadel.ward as Castle).wall.gates : null
                    ).Select(x => x.vec).Contains(new Point(end).vec))
                {
                    // shorten wall
                    end = start + startToEnd.normalized * (dis - gateOpeningSize / 2);

                    // add gate tower
                    Instantiate(gatePrefab, transform.TransformPoint(end),
                        Quaternion.Euler(90, 0, 0),
                        this.transform
                    );
                }

                if (cityModel.wall.gates.Concat(
                    cityModel.citadel != null ? (cityModel.citadel.ward as Castle).wall.gates : null
                    ).Select(x => x.vec).Contains(new Point(start).vec))
                {
                    // shorten wall
                    start = end + endToStart.normalized * (dis - gateOpeningSize / 2);

                    // add gate tower
                    Instantiate(gatePrefab, transform.TransformPoint(start),
                    Quaternion.Euler(90, 0, 0),
                    this.transform
                    );
                }

                var vec = end - start;
                var pos = Vector2.Lerp(start, end, 0.5f);
                var wall = Instantiate(wallPrefab, this.transform);

                wall.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(vec.y, vec.x) * Mathf.Rad2Deg);
                wall.transform.position = transform.TransformPoint(pos);
                wall.transform.localScale = new Vector3(dis, 1, 22);

            }

            foreach (var tower in cityModel.wall.towers)
            {
                if (cityModel.citadel != null)
                {
                    if ((cityModel.citadel.ward as Castle).wall.gates.Select(x => x.vec).Contains(tower.vec))
                    {
                        continue;
                    }
                }

                var t = Instantiate(towerPrefab, transform.TransformPoint(tower.vec), Quaternion.Euler(90, 0, 0), this.transform);
            }

            // foreach (var gate in cityModel.wall.gates)
            // {
            //     var t = Instantiate(gatePrefab, transform.TransformPoint(gate.vec), Quaternion.Euler(0, 0, 0), this.transform);
            // }

            // castle wall and towers
            if (cityModel.citadel != null)
            {
                var castle = cityModel.citadel.ward as Castle;

                for (var i = 0; i < castle.wall.shape.Count; i++)
                {
                    // wall
                    var start = castle.wall.shape[i].vec;
                    var end = castle.wall.shape[(i + 1) % castle.wall.shape.Count].vec;

                    if (cityModel.wall.shape.findEdge(castle.wall.shape[(i + 1) % castle.wall.shape.Count], castle.wall.shape[i]) == -1)
                    {
                        var endToStart = start - end;
                        var dis = endToStart.magnitude;

                        var vec = end - start;
                        var pos = Vector2.Lerp(start, end, 0.5f);
                        var wall = Instantiate(wallPrefab, this.transform);

                        wall.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(vec.y, vec.x) * Mathf.Rad2Deg);
                        wall.transform.position = transform.TransformPoint(pos);
                        wall.transform.localScale = new Vector3(dis, 1, 22);
                    }
                }

                foreach (var tower in castle.wall.towers)
                {
                    var t = Instantiate(towerPrefab, transform.TransformPoint(tower.vec), Quaternion.Euler(90, 0, 0), this.transform);
                }
            }
        }

        // roads 

        // foreach (var street in cityModel.streets)
        // {
        //     var roadControl = Instantiate(roadPrefab, this.transform.position, Quaternion.identity, this.transform).GetComponent<RoadController>();
        //     roadControl.points = street.Select(x => transform.TransformPoint(x.vec)).ToList();
        // }

        foreach (var street in cityModel.roads)
        {
            var roadControl = Instantiate(roadPrefab, this.transform.position, Quaternion.identity, this.transform).GetComponent<RoadController>();
            roadControl.points = street.Select(x => transform.TransformPoint(x.vec)).ToList();
            roadControl.roadWidth = 4;
        }

        MovePlayerToSpawnPoint(transform.TransformPoint(cityModel.roads[0][1].vec));

        var edges = new List<Polygon>();

        foreach (var patch in cityModel.inner)
        {
            patch.shape.forEdge((start, end) =>
            {
                if (cityModel.wall == null || !cityModel.wall.bordersBy(patch, start, end))
                {

                    if (!edges.Any(x => x.findEdge(start, end) != -1) && !edges.Any(x => x.findEdge(end, start) != -1))
                    {
                        edges.Add(new Polygon { start, end });
                    }
                }
            });
        }

        foreach (var edge in edges)
        {
            var roadControl = Instantiate(roadPrefab, this.transform.position, Quaternion.identity, this.transform).GetComponent<RoadController>();
            roadControl.points = new List<Vector3> { transform.TransformPoint(edge[0].vec), transform.TransformPoint(edge[1].vec) };
            roadControl.roadWidth = 8;
        }

        // foreach (var street in cityModel.arteries)
        // {
        //     var roadControl = Instantiate(roadPrefab, this.transform.position, Quaternion.identity, this.transform).GetComponent<RoadController>();
        //     roadControl.points = street.Select(x => transform.TransformPoint(x.vec)).ToList();
        // }

        // init nav mesh
        // navGrid = Instantiate(navGridPrefab, transform.position, Quaternion.identity).GetComponent<Nav2D>();

        // navGrid.transform.SetParent(this.gameObject.transform);

        //navGrid.GenerateNavMesh();

        StartCoroutine("GenerateNav2d");
    }


    // public class EdgeComparer : IEqualityComparer<Polygon>
    // {
    //     public bool Equals(Polygon x, Polygon y)
    //     {
    //         return (x[0].vec == y[0].vec && x[1].vec == y[1].vec);
    //     }

    //     public int GetHashCode(Polygon obj)
    //     {
    //         return obj.GetHashCode();
    //     }
    // }

    void Update()
    {


        if (cityModel != null)
        {

            var model = cityModel;

            if (debugDraw)
            {
                foreach (var patch in model.patches)
                {

                    foreach (var g in patch.ward.geometry)
                    {
                        for (var i = 0; i < g.Count; i++)
                        {
                            Debug.DrawLine(g[i].vec, g[(i + 1) % g.Count].vec, Color.blue);
                        }
                    }

                    for (var i = 0; i < patch.shape.Count; i++)
                    {
                        Debug.DrawLine(patch.shape[i].vec, patch.shape[(i + 1) % patch.shape.Count].vec, Color.green);
                    }
                }

                for (var i = 0; i < model.border.shape.Count; i++)
                {
                    var p1 = model.border.shape[i];
                    var p2 = model.border.shape[(i + 1) % model.border.shape.Count];
                    Debug.DrawLine(p1.vec, p2.vec, Color.blue);
                }
            }
        }
    }
}
