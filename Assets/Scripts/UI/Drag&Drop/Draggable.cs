using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Scripts.UI
{


    [System.Serializable]
    public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private bool dragging;
        Canvas canvas;
        private DropZone currentZoneBelow;

        // zone that the draggable is in
        private DropZone currentZone;

        public object data;

        public void OnBeginDrag(PointerEventData eventData)
        {
            dragging = Input.GetMouseButton(0);
            if (dragging)
            {

                // become a sibling of our slot's parent, so we're no longer part of the container (and don't disrupt the GridLayout)
                Transform p = transform.parent;
                while (p.GetComponent<Canvas>() == null && p != null)
                    p = p.parent;
                transform.SetParent(p);
                // move this to the very front of the UI, so the dragged element draws over everything
                transform.SetAsLastSibling();

                // lazy initialisation to find the canvas we're a part of   
                if (canvas == null)
                {
                    Transform t = transform;
                    while (t != null && canvas == null)
                    {
                        t = t.parent;
                        canvas = t.GetComponent<Canvas>();
                    }
                }
                // move that canvas forwards, so we're dragged on top of other items, and not behind them
                if (canvas)
                    canvas.sortingOrder = 1;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {

            if (!dragging)
            {
                return;
            }

            // move the dragged object with the mouse

            transform.position = eventData.position;

            DropZone zone = GetDropZoneUnderMouse();

            Debug.Log(zone);

            if (zone != currentZoneBelow)
            {
                if (zone != null)
                {
                    zone.OnDraggingEnter();
                }

                if (currentZoneBelow != null)
                {
                    currentZoneBelow.OnDraggingExit();
                }

                currentZoneBelow = zone;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!dragging)
                return;

            if (currentZoneBelow != null && currentZoneBelow.CanDrop(this))
            {
                AddDraggableToZone(currentZoneBelow);
            }
            else
            {
                // drag failed. Return to old zone;
                transform.SetParent(currentZone.transform);
                // move this to the very front of the UI, so the dragged element draws over everything
                transform.SetAsLastSibling();


                currentZone.OnDragFailed(this);
            }

            dragging = false;
        }

        public void AddDraggableToZone(DropZone zone)
        {
            zone.OnDraggingExit();
            currentZone = zone;
            transform.SetParent(zone.transform);
            transform.SetAsLastSibling();
            zone.OnDrop(this);
        }

        // finds the firstSlot component currently under the mouse
        private DropZone GetDropZoneUnderMouse()
        {
            PointerEventData pointer = new PointerEventData(EventSystem.current);
            pointer.position = Input.mousePosition;
            List<RaycastResult> hits = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointer, hits);
            foreach (RaycastResult hit in hits)
            {
                DropZone zone = hit.gameObject.GetComponent<DropZone>();
                DropZoneExtention zoneExtention = hit.gameObject.GetComponent<DropZoneExtention>();

                if (zoneExtention != null)
                {
                    return zoneExtention.zone;
                }

                if (zone != null)
                    return zone;
            }
            return null;
        }


        // Start is called before the first frame update
        void Start()
        {
            currentZone = transform.GetComponentInParent<DropZone>();

            Debug.Log(transform.parent);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }


}
