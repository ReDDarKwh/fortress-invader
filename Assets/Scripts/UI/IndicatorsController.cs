using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class IndicatorsController : MonoBehaviour
{
    public List<IndicatorController> indicators = new List<IndicatorController>();
    private Stack<IndicatorController> indicatorsStack = new Stack<IndicatorController>();

    void Update()
    {


        if (indicatorsStack.Count > 0)
        {

            Debug.Log(indicatorsStack.Select(x => x.name).ToString());
            var indicator = indicatorsStack.Peek();


            Debug.Log("Current indicator", indicator);

            if (!indicator.requestingHide)
            {
                Debug.Log("Current indicator not requesting hide", indicator);
                if (!indicator.visible)
                {
                    Debug.Log("Current indicator not visible", indicator);
                    indicators.ForEach(x => x.Hide());
                    indicator.Show();
                }
            }
            else
            {

                Debug.Log("Current indicator requesting hide", indicator);
                if (indicator.visible)
                {
                    Debug.Log("Current indicator visible. need hiding", indicator);
                    indicator.Hide();
                }
                else
                {
                    Debug.Log("Current indicator popping", indicator);
                    indicatorsStack.Pop();
                }
            }
        }
    }

    internal void Show(IndicatorController indicatorController)
    {
        if (!indicatorsStack.Contains(indicatorController))
        {
            Debug.Log("Adding indicator to stack ", indicatorController);
            this.indicatorsStack.Push(indicatorController);
        }
    }
}
