using UnityEngine;

public abstract class IndicatorController : MonoBehaviour
{
    public float showTime;
    [System.NonSerialized]
    public float start;
    public IndicatorsController indicatorsController;
    public abstract void Show(ActiveLinking activeLinking);
    public Animator animator;

    void Update()
    {
        if (animator.GetBool("visible"))
        {
            if (showTime != 0 && Time.time - start > showTime)
            {
                HideIndicator();
            }
        }
    }

    public void ShowIndicator(ActiveLinking activeLinking)
    {
        this.start = Time.time;
        this.indicatorsController.HideAll();
        animator.SetBool("visible", true);
        this.Show(activeLinking);
    }

    public void HideIndicator()
    {
        if (animator.GetBool("visible"))
            animator.SetBool("visible", false);
    }

}
