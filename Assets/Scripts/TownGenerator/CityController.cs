using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TownGenerator.Building;
using UnityEngine;
using Random = UnityEngine.Random;

public class CityController : MonoBehaviour
{

    public CitySettings settings;


    [System.NonSerialized]
    public Model cityModel;

    void Start()
    {

        Init();
    }

    public void Init()
    {

        Random.InitState(settings.seed);
        var time = Time.time;
        cityModel = new Model(transform.position, settings.patchNum, settings.seed);
        Debug.Log(string.Format("City Generated: {0:n0}ms", (Time.time - time) * 1000));

        Random.State oldstate = Random.state;

        Random.state = oldstate;
    }


    void Update()
    {
        if (cityModel != null)
        {
            var model = cityModel;

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
