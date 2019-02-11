using System.Collections;
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

    private GameObject spellTarget;


    public GameObject circleTargetPrefab;
    public GameObject circleTargetEffectPrefab;

    public Transform mouseTransform;


    // Use this for initialization
    void Start()
    {

        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;

        spellCaster.selectedSpell.Subscribe(spell =>
        {

            if (spellTarget != null)
            {
                Destroy(spellTarget);
                spellTarget = null;
            }

            if (spell == null)
                return;

            switch (spell.spellTarget)
            {
                case SpellTarget.CIRCLE:
                case SpellTarget.AURA:

                    spellTarget = Instantiate(
                        circleTargetPrefab);

                    var circleTarget = spellTarget.GetComponent<MagicTargetCircleController>();

                    circleTarget.targetPrefab = circleTargetPrefab;
                    circleTarget.caster = spellCaster;


                    circleTarget.effects = new List<GameObject>() { circleTargetEffectPrefab }.Select(x =>
                    {
                        var effectCopy = Instantiate(x);
                        effectCopy.transform.SetParent(circleTarget.transform);
                        return effectCopy.GetComponent<ParticleSystem>();
                    }).ToList();


                    circleTarget.follow = spell.spellTarget == SpellTarget.CIRCLE ? mouseTransform : transform;
                    circleTarget.radius = 2;

                    break;

            }
        });

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


        if (spellTarget != null)
        {
            switch (spellCaster.selectedSpell.Value.spellTarget)
            {
                case SpellTarget.CIRCLE:
                case SpellTarget.AURA:

                    var magicCircleTarget = spellTarget.GetComponent<MagicTargetCircleController>();

                    if (magicCircleTarget.radius + Input.GetAxis("Mouse ScrollWheel") * 5 != magicCircleTarget.radius)
                    {
                        // update effect radiuses

                        magicCircleTarget.setRadius(
                            magicCircleTarget.radius + Input.GetAxis("Mouse ScrollWheel") * 5
                        );

                    }

                    break;
            }



            if (Input.GetMouseButtonUp(0))
            {
                var magicTarget = spellTarget.GetComponent<MagicTargetBase>();



                // if (spellCaster.currentMana.Value > 0)
                // {

                magicTarget.Cast();

                // }
                // else
                // {

                //     Debug.Log("NOT ENOUGH MANA");
                // }

            }

            // if (Input.GetKeyDown(KeyCode.P) && Time.timeScale == 0)
            // {
            //     Time.timeScale = 1;
            // }

            if (Input.GetKeyDown(KeyCode.P))
            {
                Time.timeScale = 0;
            }

        }
    }

    void LateUpdate()
    {
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(playerToMouse.y, playerToMouse.x) * Mathf.Rad2Deg);
    }
    void FixedUpdate()
    {
        rb.MovePosition(rb.position + input * this.character.GetSpeed() * Time.fixedDeltaTime);
    }
    private Vector2 GetMousePos()
    {
        var mousePos = Input.mousePosition;
        mousePos.z = -cam.transform.position.z;
        return cam.ScreenToWorldPoint(mousePos);
    }
}

