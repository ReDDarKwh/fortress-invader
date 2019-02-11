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

    public FloatReactiveProperty maxHealth;
    public ReactiveProperty<float> currentHealth = new ReactiveProperty<float>();

    // public ReactiveProperty<float> CurrentHealth;
    // public ReactiveProperty<float> MaxHealth;




    [Tooltip("difficulty to be seen")]
    [Range(0, 1)]
    public float visibility = 0.5f;

    public BaseEvent damageTakenEvent;

    private StateMachine stateMachine;



    public ReactiveProperty<bool> IsSelected;


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
    }

    // Update is called once per frame
    void Update()
    {

    }
}
