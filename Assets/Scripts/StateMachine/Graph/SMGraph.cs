using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

[CreateAssetMenu]
public class SMGraph : NodeGraph
{

    public override void OnDrop(object droppedObject, Vector2 dropPosition)
    {
        if (droppedObject is BaseState)
        {
            Node.graphHotfix = this;
            StateNode node = ScriptableObject.CreateInstance(typeof(StateNode)) as StateNode;

            var state = droppedObject as BaseState;

            node.position = dropPosition;
            node.state = state;
            node.name = state.name;
            node.graph = this;
            nodes.Add(node);
        }
        else if (droppedObject is BaseEvent)
        {

            Node.graphHotfix = this;
            LinkNode node = ScriptableObject.CreateInstance(typeof(LinkNode)) as LinkNode;

            var evt = droppedObject as BaseEvent;

            node.position = dropPosition;
            node.trigger = evt;
            node.actionType = EventActionType.TRANSITION;

            node.name = evt.name;
            node.graph = this;
            nodes.Add(node);

        }
    }

}