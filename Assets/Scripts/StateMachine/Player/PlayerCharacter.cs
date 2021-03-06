﻿using System.Collections;
using System.Collections.Generic;
using Scripts.Spells;
using UnityEngine;
using UniRx;
using System.Linq;
using System;
using BayatGames.SaveGameFree;
using System.IO;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpellCaster))]
public class PlayerCharacter : MonoBehaviour
{
    private Rigidbody2D rb;
    private Camera cam;
    private Animator camAnimator;
    private Vector2 input;
    private Vector2 playerToMouse;

    public SpellCaster spellCaster;

     public EffectContainer ec;

    public Character character;

    public float fastWalkingSpeedNoiseRadius;
    public float fastWalkingNoiseDistance;

    private Vector3 lastNoisePosition;

    private Vector2 moveVector;

    private bool specialCastKeydown;

    [System.NonSerialized]
    public bool selectingTargets;

    [System.NonSerialized]
    public Vector3 selectingTargetsStartPos;


    public float unpauseTimeIncrement = 0.2f;
    public float pauseTimeIncrement = 0.1f;

    public float pauseSpeed = 0.005f;


    private bool pause;
    private bool menuPause;

    public BoolReactiveProperty showCurrentMissionElement = new BoolReactiveProperty(false);
    public BoolReactiveProperty showMinimap = new BoolReactiveProperty(true);

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
        camAnimator = cam.GetComponent<Animator>();

        SharedSceneController.Instance.levelChanger.MenuExited += handleMenuExited;

        FetchSpells();
    }


    public void FetchSpells()
    {
        FileInfo[] files = SaveGame.GetFiles("spells");
        spellCaster.spells = files.Select(f =>
        {
            return new { spell = SaveGame.Load<SavedSpell>("spells/" + f.Name), id = f.Name };
        }).OrderBy(x => x.spell.spellName).Select(x => Spell.FromSavedSpell(x.spell, ec.effects)).ToList();
    }


    void handleMenuExited(object sender, EventArgs e)
    {
        menuPause = false;
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
            spellCaster.Cast(true);
        }

        if (Input.GetAxisRaw("Cast Spell") == 1 && !specialCastKeydown)
        {
            specialCastKeydown = true;
            spellCaster.Cast(false);
        }

        if (Input.GetAxisRaw("Cast Spell") == 0)
        {
            specialCastKeydown = false;
        }

        if (Input.GetButtonDown("Pause"))
        {
            pause = !pause;
            camAnimator.SetBool("paused", pause);

            //Time.timeScale = 0.05f;
        }

        if (Input.GetButtonDown("Menu") && !menuPause)
        {
            //Time.timeScale = 0;
            menuPause = true;
            SharedSceneController.Instance.OpenFortressMenu();

        }

        if (Input.GetButtonDown("CurrentMission"))
        {
            showCurrentMissionElement.Value = !showCurrentMissionElement.Value;
        }

        if (Input.GetButtonDown("Minimap"))
        {
            showMinimap.Value = !showMinimap.Value;
        }


        if (pause)
        {
            Time.timeScale = Mathf.Max(pauseSpeed, Time.timeScale - pauseTimeIncrement);
        }
        else
        {
            Time.timeScale = Mathf.Min(1, Time.timeScale + unpauseTimeIncrement);
        }

        if (spellCaster.selectedSpell.Value != null && spellCaster.selectedSpell.Value.spellTarget == SpellTarget.MISSILE)
        {
            if (Input.GetMouseButtonDown(0))
            {
                selectingTargetsStartPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }

            selectingTargets = Input.GetMouseButton(0);

            if (selectingTargets)
            {
                var mousePosWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var width = mousePosWorld.x - selectingTargetsStartPos.x;
                var height = mousePosWorld.y - selectingTargetsStartPos.y;
                var pos = new Vector3(width > 0 ? selectingTargetsStartPos.x : mousePosWorld.x, height > 0 ? selectingTargetsStartPos.y : mousePosWorld.y);

                spellCaster.spellTarget.GetComponent<MagicTargetMissileController>().SelectRect(pos + new Vector3(Mathf.Abs(width) / 2, Mathf.Abs(height) / 2),

                    new Vector2(Mathf.Abs(width), Mathf.Abs(height))
                );
            }
        }
    }
    void LateUpdate()
    {

        if (!pause)
        {
            moveVector = input * this.character.GetSpeed() * Time.fixedDeltaTime;

            character.moving = moveVector.magnitude > 0;
            character.lookDirection = Quaternion.Euler(0, 0, Mathf.Atan2(playerToMouse.y, playerToMouse.x) * Mathf.Rad2Deg);
            character.moveDirection = Quaternion.Euler(0, 0, Mathf.Atan2(moveVector.y, moveVector.x) * Mathf.Rad2Deg);
        }
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

