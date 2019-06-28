


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNodeEditor;



namespace XNodeEditor.Custom
{

    [CustomNodeGraphEditor(typeof(SMGraph))]
    class StateGraphEditor : NodeGraphEditor
    {
        public override void OnDropObjects(UnityEngine.Object[] objects)
        {

            Vector2 pos = NodeEditorWindow.current.WindowToGridPosition(Event.current.mousePosition);
            foreach (var droppedObject in objects)
            {

                if (droppedObject is BaseState)
                {
                    StateNode node = ScriptableObject.CreateInstance(typeof(StateNode)) as StateNode;

                    var state = droppedObject as BaseState;

                    node.position = pos;
                    node.state = state;
                    node.name = state.name;
                    CopyNode(node);

                }
                else if (droppedObject is BaseEvent)
                {
                    LinkNode node = ScriptableObject.CreateInstance(typeof(LinkNode)) as LinkNode;

                    var evt = droppedObject as BaseEvent;

                    node.position = pos;
                    node.trigger = evt;
                    node.actionType = EventActionType.TRANSITION;

                    node.name = evt.name;
                    CopyNode(node);

                }
            }

        }
    }

}

