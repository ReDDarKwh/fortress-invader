

using System;
using System.Collections;
using System.Collections.Generic;
using C5;
using UnityEngine;

namespace Scripts.AI
{

    public class Astar
    {

        // 8 way diagonal heuristic

        private const float DiagonalCost = 2.12f;
        private const float NonDiagonalCost = 3;


        private static float getHcost(Nav2dNode node, Nav2dNode end)
        {
            var dx = Mathf.Abs(node.worldPos.x - end.worldPos.x);
            var dy = Mathf.Abs(node.worldPos.y - end.worldPos.y);
            return NonDiagonalCost * (dx + dy) + (DiagonalCost - 2 * NonDiagonalCost) * Mathf.Min(dx, dy);
        }

        private static float getGCost(Nav2dNode current, Nav2dNode next)
        {
            var normalized = (current.worldPos - next.worldPos).normalized;
            return ((normalized.x == 0 || normalized.y == 0) ? NonDiagonalCost : DiagonalCost) + next.travelCost;
        }

        private class NodeCompare : IComparer<Nav2dNode>
        {

            private Dictionary<Nav2dNode, float> pCosts;

            public NodeCompare(Dictionary<Nav2dNode, float> pCosts)
            {
                this.pCosts = pCosts;
            }

            public int Compare(Nav2dNode x, Nav2dNode y)
            {
                return (pCosts[x]).CompareTo(pCosts[y]);
            }
        }

        public static System.Collections.Generic.IList<Nav2dNode> findShortestPath(Nav2dNode start, Nav2dNode end)
        {
            System.Collections.Generic.IList<Nav2dNode> result = new List<Nav2dNode>();
            Nav2dNode current = null;

            var startTime = Time.time;



            if (start == null || end == null || !start.accessible || !end.accessible)
                return null;


            var pCosts = new Dictionary<Nav2dNode, float>();
            var gCosts = new Dictionary<Nav2dNode, float>();
            var parent = new Dictionary<Nav2dNode, Nav2dNode>();
            var opened = new IntervalHeap<Nav2dNode>(new NodeCompare(pCosts), MemoryType.Normal);

            opened.Add(start);
            gCosts[start] = 0;
            pCosts[start] = getHcost(start, end);

            while (!opened.IsEmpty)
            {

                // timout
                if (Time.time - startTime > 5)
                {
                    return null;
                }

                current = opened.DeleteMin();

                if (current == end)
                    break;

                foreach (var neighbor in current.getNeighbors())
                {


                    // add neighbor or not to the openedQueue.
                    if (neighbor.accessible)
                    {
                        float newCost;
                        float hCost;
                        lock (neighbor)
                        {
                            newCost = gCosts[current] + getGCost(current, neighbor);
                            hCost = getHcost(neighbor, end);
                        }

                        if (!gCosts.ContainsKey(neighbor) || newCost < gCosts[neighbor])
                        {
                            gCosts[neighbor] = newCost;
                            pCosts[neighbor] = gCosts[neighbor] + hCost;

                            opened.Add(neighbor);
                            parent[neighbor] = current;
                        }
                    }
                }
            }

            do
            {
                result.Add(current);
            } while (parent.TryGetValue(current, out current));

            return result;
        }

    }
}