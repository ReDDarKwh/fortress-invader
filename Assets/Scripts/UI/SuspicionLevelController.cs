using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SuspicionLevelController : IndicatorController
{

    private float suspiciousTime;
    public Image childImage;

    void Update()
    {
        childImage.fillAmount = (Time.time - start) / suspiciousTime;
    }

    public override void Init(ActiveLinking activeLinking)
    {
        start = Time.time;
        childImage.fillAmount = 0;
        suspiciousTime = activeLinking.GetValueOrDefault<float>("time");
    }

}
