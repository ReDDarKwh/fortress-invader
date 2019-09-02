using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.AI;
using Scripts.NPC;
using static FortressSceneManager;

[CreateAssetMenu(fileName = "DeadState", menuName = "StateMachine/States/DeadState")]
public class DeadState : BaseState
{

    public DeadState()
    {
        this.stateName = "dead";
    }

    public override void Enter(StateMachine stateMachine, EventMessage eventResponse, ActiveLinking activeLinking)
    {

        stateMachine.GetComponent<Animator>()?.SetBool("dead", true);
        //temporary
        //Destroy(stateMachine.gameObject);

        var sceneManager = GameObject
        .FindGameObjectWithTag("SceneManager")
        .GetComponent<FortressSceneManager>();
        var nonPlayerCharacter = stateMachine.GetComponent<NonPlayerCharacter>();

        if (nonPlayerCharacter != null)
        {
            // disable agent2d for pushing around
            var agent = stateMachine.GetComponent<Nav2DAgent>();
            if (agent != null)
                agent.enabled = false;


        }
        else
        {
            var playerCharacter = stateMachine.GetComponent<PlayerCharacter>();
            if (playerCharacter != null)
            {
                sceneManager.GameOver(GameOverType.playerDied);
            }
        }



    }

    public override void Leave(StateMachine stateMachine)
    {

    }

    public override void StateUpdate(StateMachine stateMachine, ActiveLinking activeLinking)
    {

    }


}
