using UnityEngine;

public abstract class IndicatorController : MonoBehaviour
{
    public float showTime;
    [System.NonSerialized]
    public float start;
    public IndicatorsController indicatorsController;
    public abstract void Init(ActiveLinking activeLinking);
    public Animator animator;
    public bool visible;
    public bool requestingHide;

    void Start()
    {
        indicatorsController.indicators.Add(this);
    }

    void Update()
    {
        if (visible)
        {
            animator.SetBool("visible", visible);
            if (showTime != 0 && Time.time - start > showTime)
            {
                HideIndicator();
            }
        }
    }

    public void Show()
    {
        start = Time.time;
        visible = true;
    }

    public void Hide()
    {
        visible = false;
        animator.SetBool("visible", visible);
    }

    public void ShowIndicator(ActiveLinking activeLinking)
    {
        this.requestingHide = false;
        this.indicatorsController.Show(this);
        this.Init(activeLinking);
    }

    public void HideIndicator()
    {
        this.requestingHide = true;
    }
}
