using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Scripts.AI;
using TownGenerator.Building;
using UnityEngine;
using Random = UnityEngine.Random;

public class CityController : MonoBehaviour
{

    public CitySettings settings;

    public bool debugDraw = false;

    [System.NonSerialized]
    public Model cityModel;


    public GameObject building;
    public GameObject navGridPrefab;

    private Nav2D navGrid;



    private IEnumerator coroutine;

    void Start()
    {





        Init();
    }


    private IEnumerator GenerateNav2d()
    {
        navGrid.GenerateNavMesh();
        yield return new WaitForSecondsRealtime(0.1f);
    }

    public void Init()
    {

        //create the city data model
        Random.InitState(settings.seed);
        cityModel = new Model(settings.patchNum, settings.seed);
        Random.State oldstate = Random.state;
        Random.state = oldstate;

        //create buildings

        foreach (var patch in cityModel.patches)
        {
            foreach (var shape in patch.ward.geometry)
            {

                var b = Instantiate(building, this.transform, false);
                b.transform.rotation = Quaternion.identity;
                //b.transform.position += transform.TransformPoint((Vector3)cityModel.center.vec);

                Debug.DrawLine(transform.position, cityModel.center.vec, Color.magenta, 20);

                var buildingController = b.GetComponent<BuildingController>();
                buildingController.shape = shape;
            }
        }


        // init nav mesh
        navGrid = Instantiate(navGridPrefab, transform.position, Quaternion.identity).GetComponent<Nav2D>();

        navGrid.transform.SetParent(this.gameObject.transform);
        navGrid.transform.position -= new Vector3(navGrid.width / 2 * navGrid.grid.cellSize.x, navGrid.height / 2 * navGrid.grid.cellSize.y);

        //navGrid.GenerateNavMesh();
    }


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
