using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using Scripts.Characters;

public class LinkNode : Node
{

    // Use this for initialization
    [Input] public List<string> tags;
    [Input] public List<BaseState> from;
    [Output] public List<BaseState> to;


    public BaseEvent trigger;
    public EventActionType actionType;
    public bool invert = false;


    protected override void Init()
    {
        base.Init();

    }

    // Return the correct value of an output port when requested
    public override object GetValue(NodePort port)
    {
        return this;
    }
}