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
            var indicator = indicatorsStack.Peek();
            if (!indicator.requestingHide)
            {
                if (!indicator.visible)
                {
                    indicators.ForEach(x => x.Hide());
                    indicator.Show();
                }
            }
            else
            {
                if (indicator.visible)
                {
                    indicator.Hide();
                }
                else
                {
                    indicatorsStack.Pop();
                }
            }
        }
    }

    internal void Show(IndicatorController indicatorController)
    {
        if (!indicatorsStack.Contains(indicatorController))
        {
            this.indicatorsStack.Push(indicatorController);
        }
    }
}
