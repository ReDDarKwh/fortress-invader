using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class IndicatorsController : MonoBehaviour
{

    private IndicatorController[] indicators;

    // Use this for initialization
    void Start()
    {
        indicators = GetComponentsInChildren<IndicatorController>();
    }

    public void HideAll()
    {
        foreach (var indicator in indicators)
        {
            indicator.HideIndicator();
        }
    }
}
