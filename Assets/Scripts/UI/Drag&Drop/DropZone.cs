using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Scripts.UI
{


    [System.Serializable]
    public class DropEvent : UnityEvent<Draggable> { }


    [System.Serializable]
    public class DropFailedEvent : UnityEvent<Draggable> { }



    public class DropZone : MonoBehaviour
    {

        public UnityEvent onDragEnter;
        public UnityEvent onDragExit;
        public DropEvent onDrop;

        public DropFailedEvent onDragFailed;


        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        internal void OnDraggingEnter()
        {
            onDragEnter.Invoke();
        }

        internal void OnDraggingExit()
        {
            onDragExit.Invoke();
        }

        internal void OnDrop(Draggable draggable)
        {
            onDrop.Invoke(draggable);
        }

        internal void OnDragFailed(Draggable draggable)
        {
            onDragFailed.Invoke(draggable);
        }
    }
}
