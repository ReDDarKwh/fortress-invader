using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.Spells;
using UniRx;

public class SpellSlotController : MonoBehaviour
{


    public Color selectedColor;
    public Color unSelectedColor;
    public Image slotImage;

    [System.NonSerialized]
    public Spell spell;

    [System.NonSerialized]
    public int number;


    [System.NonSerialized]
    public SpellCaster spellCaster;

    public void OnSelect()
    {
        spellCaster.selectedSpell.Value = spell;
    }

    // Use this for initialization
    void Start()
    {
        spellCaster.selectedSpell.Subscribe(x =>
        {
            if (x == spell)
            {
                slotImage.color = selectedColor;
            }
            else
            {
                slotImage.color = unSelectedColor;
            }
        });

        AlchemyCircle circle = GetComponentInChildren<AlchemyCircle>();

        circle.seed = spell?.spellName.GetHashCode() ?? 0;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(number.ToString()))
        {
            OnSelect();
        }
    }
}
