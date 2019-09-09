using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DragAndDropUtil;

public class PowersUI : ObjectContainerList<Power>
{

    public Player player;

    // Use this for initialization
    void Start()
    {
        CreateSlots(player.powers);
    }
}
