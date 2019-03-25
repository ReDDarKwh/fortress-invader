using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Globalization;
using Random = UnityEngine.Random;

namespace Scripts.AI
{
    public class Nav2dNode
    {
        private C5.HashSet<Nav2dNode> neighbors;
        //if the  direction can be traveled or not
        public bool accessible;
        public bool border;
        public Vector3 worldPos;
        public float travelCost = 0;

        private int walkableConnectionNum = 0;
        public Nav2dNode parent;

        public Color zone;
        public Nullable<Color> givenZone;

        public Nav2dNode(Vector3 worldPos)
        {
            this.worldPos = worldPos;
            this.neighbors = new C5.HashSet<Nav2dNode>();

            zone = Random.ColorHSV();
        }

        // sets connection two ways, node <-> node
        public void setAccessibility(Nav2dNode[] nodes, int layerMask)
        {
            foreach (var neighbor in nodes)
            {
                //if input neighbor empty or neightbor already set SKIP
                if (neighbor == null || neighbors.Contains(neighbor))
                    continue;

                var hit = Physics2D.Raycast(worldPos, (neighbor.worldPos - worldPos), (neighbor.worldPos - worldPos).magnitude, layerMask);

                if (hit.collider == null)
                {
                    walkableConnectionNum++;
                    neighbor.walkableConnectionNum++;
                    neighbor.zone = this.zone;

                    // add the neighbor to the list
                    neighbors.Add(neighbor);
                    // add this node to the neightbor
                    neighbor.neighbors.Add(this);
                }

            }
            this.accessible = walkableConnectionNum >= 1;

        }


        public IEnumerable<Nav2dNode> getNeighbors()
        {
            lock (neighbors)
                return new List<Nav2dNode>(neighbors).Where(x => x != null);
        }

        internal void drawConnections()
        {


            // if (!this.accessible)
            //     return;
            foreach (var neighbor in neighbors)
            {
                if (neighbor == null)
                    continue;


                Debug.DrawLine(
                                    this.worldPos + (neighbor.worldPos - this.worldPos).normalized / 5,
                                    neighbor.worldPos + (this.worldPos - neighbor.worldPos).normalized / 5,
                                    neighbor.accessible ? zone : new Color(1, 0, 0, 0.2f)
                                );

            }
        }
    }
}