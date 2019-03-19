﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Scripts.Spells;





public class PlayerHUDController : MonoBehaviour
{

    // private class SpellSlot
    // {
    //     public Spell spell;
    //     public Selectable slot;

    //     public selected
    // }

    public Image playerHealthBar;
    public Image playerManaBar;
    public PlayerCharacter playerCharacter;

    public GameObject spellsBar;

    public GameObject spellSlotPrefab;

    public GameObject spellMouseUI;

    public GameObject castSpellText;

    public GameObject spellSelectionImage;


    private float vel = 0.0f;


    //rivate List<SpellSlot>

    // Use this for initialization
    void Start()
    {
        playerCharacter.spellCaster.currentMana.Subscribe(x =>
        {
            playerManaBar.fillAmount = x / playerCharacter.spellCaster.maxMana.Value;
        });


        playerCharacter.spellCaster.selectedSpell.Subscribe(x =>
        {
            switch (x?.spellTarget)
            {
                case SpellTarget.MISSILE:
                    spellMouseUI.SetActive(true);
                    castSpellText.SetActive(true);

                    break;

                default:
                    spellMouseUI.SetActive(false);
                    castSpellText.SetActive(false);

                    break;
            }
        });


        for (var i = 1; i <= playerCharacter.spellCaster.spells.Count; i++)
        {
            createSpellSlot(i, playerCharacter.spellCaster.spells[i - 1]);
        }

        // create "no spell" slot
        createSpellSlot(0, null);
    }


    private void createSpellSlot(int number, Spell spell)
    {
        var slot = Instantiate(spellSlotPrefab, Vector3.zero, Quaternion.identity);
        var slotController = slot.GetComponent<SpellSlotController>();

        slotController.spell = spell;
        slotController.number = number;
        slotController.spellCaster = playerCharacter.spellCaster;
        slot.transform.SetParent(spellsBar.transform);
    }

    // Update is called once per frame
    void Update()
    {

        playerHealthBar.fillAmount = Mathf.SmoothDamp(
            playerHealthBar.fillAmount,
            playerCharacter.character.currentHealth.Value / playerCharacter.character.maxHealth.Value,
            ref vel, 0.1f
       );


        // missile number selected display logic
        if (playerCharacter.spellCaster.selectedSpell.Value != null && playerCharacter.spellCaster.selectedSpell.Value.spellTarget == SpellTarget.MISSILE)
        {
            var missileTarget = playerCharacter.spellCaster.spellTarget.GetComponent<MagicTargetMissileController>();
            var missileTargetSelectedText = spellMouseUI.GetComponent<Text>();

            missileTargetSelectedText.text = $"{missileTarget.selectedCharacters.Count}/{missileTarget.maxTargets}";
            spellMouseUI.transform.position = Input.mousePosition;
        }


        // missile selection box logic
        if (playerCharacter.selectingTargets)
        {
            spellSelectionImage.SetActive(true);

            var rectTransform = spellSelectionImage.GetComponent<RectTransform>();


            var selectingViewportPos = Camera.main.WorldToScreenPoint(playerCharacter.selectingTargetsStartPos);

            var width = Input.mousePosition.x - selectingViewportPos.x;
            var height = selectingViewportPos.y - Input.mousePosition.y;

            rectTransform.position = selectingViewportPos;

            rectTransform.position = new Vector2(width > 0 ? selectingViewportPos.x : Input.mousePosition.x,

             height > 0 ? selectingViewportPos.y : Input.mousePosition.y);

            rectTransform.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
        }
        else
        {
            spellSelectionImage.SetActive(false);
        }

    }


}