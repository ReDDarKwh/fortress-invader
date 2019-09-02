using UnityEngine;

public abstract class IndicatorController : MonoBehaviour
{
    public float showTime;
    [System.NonSerialized]
    public float start;
    public IndicatorsController indicatorsController;
    public abstract void Init(ActiveLinking activeLinking);
    public abstract void ChildUpdate();

    public Animator animator;
    public bool visible;
    public bool requestingHide;

    void Start()
    {
        indicatorsController.indicators.Add(this);
    }

    void Update()
    {
        Debug.Log($"Updating visible:{visible}", this);
        if (visible)
        {
            animator.SetBool("visible", visible);
            Debug.Log("Showing", this);
            if (showTime != 0 && Time.time - start > showTime)
            {
                HideIndicator();
            }

            ChildUpdate();
        }
    }

    public void Show()
    {

        Debug.Log("Running show", this);
        start = Time.time;
        visible = true;
    }

    public void Hide()
    {
        visible = false;
        animator.SetBool("visible", visible);
        Debug.Log("Hiding", this);
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
