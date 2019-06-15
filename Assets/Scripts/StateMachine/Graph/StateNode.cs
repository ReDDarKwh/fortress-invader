using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class StateNode : Node
{

    [Output] public BaseState state;

    [Input] public List<LinkNode> links;

    // Use this for initialization
    protected override void Init()
    {
        base.Init();

    }

    // Return the correct value of an output port when requested
    public override object GetValue(NodePort port)
    {
        if (port.fieldName == "state")
        {
            return state;
        }

        return null;
    }
}