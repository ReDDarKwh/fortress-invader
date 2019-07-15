using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CamController : MonoBehaviour
{

    public Character follow;
    public Vector3 offset;
    private Camera cam;
    public float zoomIncrement;

    // Use this for initialization
    void Start()
    {
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        PosUpdate();
        ZoomUpdate();
    }

    private void ZoomUpdate()
    {
        if (Input.GetKeyUp(KeyCode.Minus))
        {
            cam.orthographicSize += zoomIncrement;
        }

        if (Input.GetKeyUp(KeyCode.Equals))
        {
            cam.orthographicSize = Math.Max(5, cam.orthographicSize - zoomIncrement);
        }
    }

    private void PosUpdate()
    {
        Vector3 camPos;
        // space = mouse follow mode, else follow default follower
        if (Input.GetAxisRaw("Camera Movement") == 1)
        {

            var mousePos = Input.mousePosition;
            mousePos.z = -cam.transform.position.z;

            camPos = follow.transform.position + ((cam.ScreenToWorldPoint(mousePos) - follow.transform.position) / 2);



        }
        else
        {
            camPos = follow.transform.position;
        }


        cam.transform.position = camPos + offset;
    }

}
