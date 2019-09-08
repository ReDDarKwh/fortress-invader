using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BayatGames.SaveGameFree;
using Scripts.Spells;

public class SpellbookUIController : MonoBehaviour
{

    public GameObject spellElementPrefab;
    public GameObject spellbookContainerElement;
    public List<Spell> spellbook;
    public SpellEffect spellEffect;

    // Start is called before the first frame update
    void Start()
    {
        spellbook = SaveGame.Exists("spellbook") ?
            SaveGame.Load<List<Spell>>("spellbook") : new List<Spell>();

        foreach (var spell in spellbook)
        {
            var spellElement = Instantiate(spellElementPrefab, spellbookContainerElement.transform)
            .GetComponent<SpellUIElementController>();
            spellElement.spell = spell;
        }

        Debug.Log(spellbook);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
