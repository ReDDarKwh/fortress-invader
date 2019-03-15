using System.Collections;
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


    private float vel = 0.0f;


    //rivate List<SpellSlot>

    // Use this for initialization
    void Start()
    {
        playerCharacter.spellCaster.currentMana.Subscribe(x =>
        {
            playerManaBar.fillAmount = x / playerCharacter.spellCaster.maxMana.Value;
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





    }
}
