using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BayatGames.SaveGameFree;
using Scripts.Spells;

public class SpellbookUIController : MonoBehaviour
{

    public GameObject spellElementPrefab;
    public List<Spell> spellbook;

    // Start is called before the first frame update
    void Start()
    {
        spellbook = SaveGame.Load<List<Spell>>("spellbook");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
