using System.Collections;
using System.Collections.Generic;
using Scripts.Spells;
using UnityEngine;
using UnityEngine.UI;

public class SpellUIElementController : MonoBehaviour
{
    public Text spellName;
    public Text manaCost;
    public Text createCost;
    public Image spellImage;
    public Spell spell;



    // Start is called before the first frame update
    void Start()
    {
        spellName.text = spell.spellName;
        manaCost.text = $"Costs <b>{spell.manaCost}</b> to cast";
        createCost.text = $"Costs <b>{spell.createCost}</b> to cast";
    }

    // Update is called once per frame
    void Update()
    {

    }
}
