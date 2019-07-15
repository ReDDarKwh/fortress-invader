using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class StateGroupNode : Node
{

    [Output]
    public List<string> tags;

    public List<string> _tags;


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
        if (port.fieldName == "tags")
        {
            return _tags;
        }; // Replace this

        return null;
    }

}