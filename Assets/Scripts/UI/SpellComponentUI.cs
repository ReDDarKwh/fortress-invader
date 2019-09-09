using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DragAndDropUtil;
using Scripts.Spells;


public class SpellComponentUI : Draggable
{


    public override void UpdateObject()
    {
        SpellEffect item = obj as SpellEffect;

        gameObject.SetActive(item != null);
    }
}
