using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleRenderer : MonoBehaviour
{
    public float radius;
    public int pointNumber;
    public LineRenderer lineRenderer;
    // Use this for initialization
    void Start()
    {
        //lineRenderer = GetComponent<LineRenderer>();

        UpdateCircle();
    }


    public void UpdateCircle()
    {

        var positions = new List<Vector3>();

        for (var i = 1; i <= pointNumber; i++)
        {
            positions.Add(
                new Vector3(
                    Mathf.Cos(Mathf.PI * 2 / pointNumber * i) * radius,
                    Mathf.Sin(Mathf.PI * 2 / pointNumber * i) * radius
                )
            );
        }

        lineRenderer.positionCount = pointNumber;
        lineRenderer.SetPositions(positions.ToArray());
    }

    // Update is called once per frame
    void Update()
    {




    }



}
