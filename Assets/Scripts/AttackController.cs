using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour
{

    public Animator animator;
    public float attackRadius;
    public int damage;
    public string[] affectedLayers;
    public float attackNoiseRadius;


    void Start()
    {

    }

    public void DealDamage()
    {
        var colliders = Physics2D.OverlapCircleAll(this.transform.position, attackRadius, LayerMask.GetMask(affectedLayers));
        foreach (var c in colliders)
        {

            if (transform.IsChildOf(c.transform))
                continue;

            var character = c.GetComponent<Character>();
            if (character)
            {

                character.noiseEmitter.EmitNoise(attackNoiseRadius);

                character.AddDamage(damage);
            }
        }
    }

    // public void UpdateAttack(ActiveLinking linking)
    // {
    //     if (Time.time - lastAttackTime > attackCooldown)
    //     {
    //         Attack(linking);
    //         lastAttackTime = Time.time;
    //     }
    // }

    public void Attack()
    {
        animator.SetTrigger("swing");
    }
}
