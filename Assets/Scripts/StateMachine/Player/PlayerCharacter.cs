﻿using System.Collections;
using System.Collections.Generic;
using Scripts.Spells;
using UnityEngine;
using UniRx;
using System.Linq;


[RequireComponent(typeof(GameObject))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpellCaster))]
public class PlayerCharacter : MonoBehaviour
{
    private Rigidbody2D rb;
    private Camera cam;

    private Vector2 input;
    private Vector2 playerToMouse;

    public SpellCaster spellCaster;

    public Character character;


    public float fastWalkingSpeedNoiseRadius;
    public float fastWalkingNoiseDistance;

    private Vector3 lastNoisePosition;


    private Vector2 moveVector;


    // Use this for initialization
    void Start()
    {

        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;

    }

    // Update is called once per frame
    void Update()
    {
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        playerToMouse = GetMousePos() - rb.position;


        if (Input.GetAxisRaw("Fast Speed") == 1)
        {
            character.currentSpeed = CharacterSpeed.FAST;
        }
        else
        {

            character.currentSpeed = CharacterSpeed.NORMAL;
        }


        if (character.currentSpeed == CharacterSpeed.FAST)
        {
            if ((lastNoisePosition - transform.position).magnitude > fastWalkingNoiseDistance)
            {
                lastNoisePosition = transform.position;
                character.noiseEmitter.EmitNoise(fastWalkingSpeedNoiseRadius);
            }
        }


        // change radius for area spells
        if (spellCaster.spellRadius + Input.GetAxis("Mouse ScrollWheel") * 5 != spellCaster.spellRadius)
        {
            // update effect radiuses
            spellCaster.spellRadius = spellCaster.spellRadius + Input.GetAxis("Mouse ScrollWheel") * 5;
        }

        // cast spell on click;
        if (Input.GetMouseButtonUp(0))
        {
            spellCaster.Cast();
        }

        if (Input.GetAxisRaw("Pause") == 1)
        {
            Time.timeScale = 0.05f;
        }
        else
        {
            Time.timeScale = 1;
        }

    }

    void LateUpdate()
    {
        moveVector = input * this.character.GetSpeed() * Time.fixedDeltaTime;

        character.moving = moveVector.magnitude > 0;
        character.lookDirection = Quaternion.Euler(0, 0, Mathf.Atan2(playerToMouse.y, playerToMouse.x) * Mathf.Rad2Deg);
        character.moveDirection = Quaternion.Euler(0, 0, Mathf.Atan2(moveVector.y, moveVector.x) * Mathf.Rad2Deg);
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveVector);
    }
    private Vector2 GetMousePos()
    {
        var mousePos = Input.mousePosition;
        mousePos.z = -cam.transform.position.z;
        return cam.ScreenToWorldPoint(mousePos);
    }
}

