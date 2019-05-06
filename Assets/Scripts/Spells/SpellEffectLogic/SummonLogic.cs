using System.Collections;
using System.Collections.Generic;
using Scripts.Spells;
using UnityEngine;


[CreateAssetMenu(fileName = "SummonLogic", menuName = "Spells/EffectLogic/SummonLogic")]
public class SummonLogic : BaseState
{

    public GameObject summonedPrefab;

    // stateMachine and activeLinking always null
    public override void Enter(StateMachine stateMachine, EventMessage eventResponse, ActiveLinking activeLinking)
    {

        var spellTarget = eventResponse.target.GetComponent<MagicTargetBase>();

        switch (spellTarget.spell.spellTarget)
        {
            case SpellTarget.CIRCLE:

                var circleTarget = eventResponse.target.GetComponent<MagicTargetCircleController>();
                var radius = circleTarget.radius;

                for (var i = 0; i < radius; i++)
                {
                    var angle = Random.Range(0, 2 * Mathf.PI);
                    var distance = Random.Range(0, radius);

                    Instantiate(
                        summonedPrefab,
                        eventResponse.target.transform.position +
                        new Vector3(Mathf.Cos(angle) * distance, Mathf.Sin(angle) * distance),
                        Quaternion.identity
                    );
                }

                break;
        }
        //temporary
        //Destroy(stateMachine.gameObject);
    }

    // unused for state
    public override void Leave(StateMachine stateMachine) { }
    public override void StateUpdate(StateMachine stateMachine, ActiveLinking activeLinking) { }
}
