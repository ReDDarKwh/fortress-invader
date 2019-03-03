using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;



public enum CharacterSpeed
{
    FAST,
    NORMAL,
    SLOW
}

[RequireComponent(typeof(StateMachine))]
[RequireComponent(typeof(NoiseEmitter))]
public class Character : MonoBehaviour
{

    public float GetSpeed()
    {
        switch (currentSpeed)
        {
            case CharacterSpeed.FAST:
                return baseMovementSpeed * fastSpeedModifier * effectSpeedModifier;
            case CharacterSpeed.SLOW:
                return baseMovementSpeed * slowSpeedModifier * effectSpeedModifier;
            default: // normal
                return baseMovementSpeed * normalSpeedModifier * effectSpeedModifier;
        }
    }

    public float effectSpeedModifier = 1;
    public float fastSpeedModifier;
    public float normalSpeedModifier;
    public float slowSpeedModifier;


    [System.NonSerialized]
    public CharacterSpeed currentSpeed = CharacterSpeed.NORMAL;

    public float baseMovementSpeed;


    [System.NonSerialized]
    public Animator animator;

    [System.NonSerialized]
    public NoiseEmitter noiseEmitter;

    [System.NonSerialized]
    public EffectIndicatorController effectIndicators;


    [System.NonSerialized]
    public Quaternion LookDirection;



    [System.NonSerialized]
    public Quaternion MoveDirection;



    public FloatReactiveProperty maxHealth;
    public ReactiveProperty<float> currentHealth = new ReactiveProperty<float>();


    public GameObject legsObject;
    public GameObject bodyObject;
    public GameObject head;



    private Animator legsAnimator;
    private Animator bodyAnimator;



    // public ReactiveProperty<float> CurrentHealth;
    // public ReactiveProperty<float> MaxHealth;




    [Tooltip("difficulty to be seen")]
    [Range(0, 1)]
    public float visibility = 0.5f;

    public BaseEvent damageTakenEvent;

    private StateMachine stateMachine;



    public ReactiveProperty<bool> IsSelected;


    private Vector3 lastPos;




    public float bodyRotationSmoothness = 0.3f;
    private float zBodyVelocity = 0.0f;

    public float legsRotationSmoothness = 0.3f;
    private float zLegsVelocity = 0.0f;



    public void AddDamage(float damage)
    {
        this.currentHealth.Value -= damage;
        //stateMachine.TriggerEvent(damageTakenEvent, EventMessage.EmptyMessage);

        // sound of getting hurt that AI might
        //soundEmitter.EmitSound(10);


        animator.ResetTrigger("hit");
        animator.SetTrigger("hit");

    }


    // Use this for initialization
    void Start()
    {


        stateMachine = GetComponent<StateMachine>();
        animator = GetComponent<Animator>();
        noiseEmitter = GetComponent<NoiseEmitter>();

        effectIndicators = GetComponentInChildren<EffectIndicatorController>();



        currentHealth.Value = maxHealth.Value;
        IsSelected = new ReactiveProperty<bool>();
        IsSelected.Subscribe(x => animator.SetBool("selected", x));


        legsAnimator = legsObject.GetComponent<Animator>();
        bodyAnimator = bodyObject.GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {

        // rotate head and body
        this.head.transform.rotation = LookDirection;


        // rotate body

        float zAngle = Mathf.SmoothDampAngle(this.bodyObject.transform.eulerAngles.z, LookDirection.eulerAngles.z, ref zBodyVelocity, bodyRotationSmoothness);

        this.bodyObject.transform.rotation = Quaternion.Euler(0, 0, zAngle);


        // rotate legs


        zAngle = Mathf.SmoothDampAngle(this.legsObject.transform.eulerAngles.z, MoveDirection.eulerAngles.z, ref zLegsVelocity, legsRotationSmoothness);

        this.legsObject.transform.rotation = Quaternion.Euler(0, 0, zAngle);





    }
}
