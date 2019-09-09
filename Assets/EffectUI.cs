using System.Collections;
using System.Collections.Generic;
using DragAndDropUtil;
using UnityEngine;
using Scripts.Spells;

public class EffectUI : ObjectContainerList<SpellEffect>
{
    public List<SpellEffect> effects;

    // Start is called before the first frame update
    void Start()
    {
        CreateSlots(effects);
    }


}
