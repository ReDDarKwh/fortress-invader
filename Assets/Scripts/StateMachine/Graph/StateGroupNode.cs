using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class StateGroupNode : Node
{

    [Output]
    public List<BaseState> states;

    [System.NonSerialized]
    public SMGraph stateMachineGraph;
    // Use this for initialization
    protected override void Init()
    {
        base.Init();
    }
    // Return the correct value of an output port when requested
    public override object GetValue(NodePort port)
    {
        return null; // Replace this
    }
}