


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XNodeEditor;
using System.Linq;



namespace XNodeEditor.Custom
{

    [CustomNodeGraphEditor(typeof(SMGraph))]
    class StateGraphEditor : NodeGraphEditor
    {

        /// <summary> Create a node and save it in the graph asset </summary>
        private void CreateNode(XNode.Node node, string name, Vector2 position)
        {
            node.position = position;
            node.name = name;

            CopyNode(node);
        }

        public override void OnDropObjects(UnityEngine.Object[] objects)
        {

            Vector2 pos = NodeEditorWindow.current.WindowToGridPosition(Event.current.mousePosition);
            foreach (var droppedObject in objects)
            {

                if (droppedObject is BaseState)
                {
                    var node = ScriptableObject.CreateInstance(typeof(StateNode)) as StateNode;
                    var state = droppedObject as BaseState;
                    node.states = new List<BaseState> { state };
                    CreateNode(node, state.name, pos);
                }
                else if (droppedObject is SMGraph)
                {
                    var node = ScriptableObject.CreateInstance(typeof(StateNode)) as StateNode;

                    var graph = droppedObject as SMGraph;

                    node.states = graph.nodes.Where(x => x is StateNode)
                    .SelectMany(x => (x as StateNode).states).Distinct().ToList();

                    CreateNode(node, graph.name, pos);
                }
                else if (droppedObject is BaseEvent)
                {
                    var node = ScriptableObject.CreateInstance(typeof(LinkNode)) as LinkNode;
                    var evt = droppedObject as BaseEvent;
                    node.trigger = evt;
                    node.actionType = EventActionType.TRANSITION;
                    CreateNode(node, evt.name, pos);
                }

            }

        }
    }

}

