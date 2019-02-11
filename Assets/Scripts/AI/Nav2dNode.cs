using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Scripts.AI
{
    public class Nav2dNode 
    {
        private Nav2dNode[] neighbors;
        //if the  direction can be traveled or not
        public bool accessible;
        public bool border;
        public Vector3 worldPos;
        public float travelCost = 0;

        public Nav2dNode parent;
        public Nav2dNode(Vector3 worldPos)
        {
            this.worldPos = worldPos;
            this.neighbors = new Nav2dNode[9];


            
        }

        public void setAccessibility(Nav2dNode[] nodes, int layerMask)
        {
            var walkableConnectionNum = 0;
            for (var y = 0; y < 9; y++)
            {
                var neighbor = nodes[y];
                if (neighbor == null)
                    continue;

                var hit = Physics2D.Raycast(worldPos, (neighbor.worldPos - worldPos), (neighbor.worldPos - worldPos).magnitude, layerMask);

                if (hit.collider == null)
                {
                    walkableConnectionNum++;
                }
                neighbors[y] = neighbor;
            }
            this.accessible = walkableConnectionNum >= 1;
            this.border = this.accessible && !(walkableConnectionNum == 8);
        }
        public IEnumerable<Nav2dNode> getNeighbors()
        {
            lock (neighbors)
                return new List<Nav2dNode>(neighbors).Where(x => x != null);
        }

        internal void drawConnections()
        {
            if (!this.accessible)
                return;
            foreach (var neighbor in neighbors)
            {
                if (neighbor == null)
                    continue;

                if (this.border && neighbor.border)
                {
                    Debug.DrawLine(
                                        this.worldPos + (neighbor.worldPos - this.worldPos).normalized / 5,
                                        neighbor.worldPos + (this.worldPos - neighbor.worldPos).normalized / 5,
                                        new Color(0, 0, 1, 0.2f)
                                    );
                }
                else
                {
                    Debug.DrawLine(
                                        this.worldPos + (neighbor.worldPos - this.worldPos).normalized / 5,
                                        neighbor.worldPos + (this.worldPos - neighbor.worldPos).normalized / 5,
                                        neighbor.accessible ? new Color(0, 1, 0, 0.2f) : new Color(1, 0, 0, 0.2f)
                                    );
                }
            }
        }
    }
}