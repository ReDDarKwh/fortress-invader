using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SuspicionLevelController : IndicatorController
{
    private bool visible;
    private float suspiciousTime;
    public Image childImage;

    // Use this for initialization


    // Update is called once per frame
    void Update()
    {
        childImage.fillAmount = (Time.time - start) / suspiciousTime;
    }

    public override void Show(ActiveLinking activeLinking)
    {
        start = Time.time;
        childImage.fillAmount = 0;
        suspiciousTime = activeLinking.GetValueOrDefault<float>("time");
        //this.gameObject.SetActive(true);
    }

}
