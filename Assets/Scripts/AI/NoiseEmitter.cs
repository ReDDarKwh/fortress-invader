using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NoiseEmitter : MonoBehaviour
{

    public BaseEvent soundHeardEvent;
    public string[] layersThatCanHear = new string[] { "AI" };

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void EmitNoise(float noiseRadius)
    {

        var colliders = Physics2D.OverlapCircleAll(transform.position, noiseRadius, LayerMask.GetMask(layersThatCanHear));

        foreach (var collider in colliders)
        {
            var stateMachine = collider.GetComponent<StateMachine>();
            var message = new EventMessage() { target = this.gameObject };
            stateMachine.TriggerEvent(soundHeardEvent, message);

            //Debug.Log("Emit noise");
        }
    }
}
