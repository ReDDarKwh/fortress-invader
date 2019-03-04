using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

using UniRx;

public class EnemyStatusController : MonoBehaviour
{


    public Character character;
    public Image healthBG;
    public Image healthBar;
    public Vector3 offset;

    private float vel = 0.0f;

    // Use this for initialization
    void Start()
    {
        // character.currentHealth.Subscribe(x =>
        // {
        //     healthBar.fillAmount = x / character.maxHealth.Value;
        // });
    }

    // Update is called once per frame
    void Update()
    {

        healthBar.fillAmount = Mathf.SmoothDamp(
           healthBar.fillAmount,
           character.currentHealth.Value / character.maxHealth.Value,
           ref vel, 0.2f
        );
    }

    void LateUpdate()
    {
        // ignore parent rotation;
        transform.forward = Vector3.forward;
        transform.position = transform.parent.position + offset;
    }
}
