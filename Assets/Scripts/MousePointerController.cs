using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousePointerController : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        var mousePos = Input.mousePosition;
        mousePos.z = -Camera.main.transform.position.z;

        transform.position = Camera.main.ScreenToWorldPoint(mousePos);
    }
}
