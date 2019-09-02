using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SuspicionLevelController : IndicatorController
{
    //private bool visible;
    private float suspiciousTime;
    public Image childImage;

    public override void ChildUpdate()
    {
        if (childImage.fillAmount < 1)
        {
            childImage.fillAmount = (Time.time - start) / suspiciousTime;
        }
    }

    public override void Init(ActiveLinking activeLinking)
    {
        start = Time.time;
        childImage.fillAmount = 0;
        suspiciousTime = activeLinking.GetValueOrDefault<float>("time");
    }

}
