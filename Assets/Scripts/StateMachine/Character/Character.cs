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
    public Quaternion lookDirection;

    [System.NonSerialized]
    public Quaternion moveDirection;

    [System.NonSerialized]
    public bool moving;






    public float maxHealth;
    public float currentHealth;


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
        this.currentHealth -= damage;
        //stateMachine.TriggerEvent(damageTakenEvent, EventMessage.EmptyMessage);

        // sound of getting hurt that AI might
        //soundEmitter.EmitSound(10);


        animator.ResetTrigger("hit");
        animator.SetTrigger("hit");

    }


    // Use this for initialization
    void Start()
    {

        currentHealth = maxHealth;

        stateMachine = GetComponent<StateMachine>();
        animator = GetComponent<Animator>();
        noiseEmitter = GetComponent<NoiseEmitter>();
        effectIndicators = GetComponentInChildren<EffectIndicatorController>();

        IsSelected = new ReactiveProperty<bool>();
        IsSelected.Subscribe(x => animator.SetBool("selected", x));

        legsAnimator = legsObject.GetComponent<Animator>();
        bodyAnimator = bodyObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        // rotate head and body
        this.head.transform.rotation = lookDirection;


        // rotate body

        float zAngle = Mathf.SmoothDampAngle(this.bodyObject.transform.eulerAngles.z, lookDirection.eulerAngles.z, ref zBodyVelocity, bodyRotationSmoothness);

        this.bodyObject.transform.rotation = Quaternion.Euler(0, 0, zAngle);



        // compare look and moving direction to choose appropriate leg animation

        var moveAndLookDirectionDifference = Quaternion.Euler(moveDirection.eulerAngles - lookDirection.eulerAngles);


        var legAngle = moveDirection.eulerAngles.z;


        if (moving)
        {
            legsAnimator.SetBool("Walking", true);
            if (
                moveAndLookDirectionDifference.eulerAngles.z >= 135
                && moveAndLookDirectionDifference.eulerAngles.z <= 225
            )
            {
                legAngle += 180;
                legsAnimator.SetBool("Reversed", true);
            }
            else
            {
                legsAnimator.SetBool("Reversed", false);
            }

            zAngle = Mathf.SmoothDampAngle(this.legsObject.transform.eulerAngles.z, legAngle, ref zLegsVelocity, legsRotationSmoothness);
        }
        else
        {

            legsAnimator.SetBool("Walking", false);
        }


        // rotate legs

        this.legsObject.transform.rotation = Quaternion.Euler(0, 0, zAngle);


        // set leg animation speed


        this.legsAnimator.speed = GetSpeed() / baseMovementSpeed;


    }
}
