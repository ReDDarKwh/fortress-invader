using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using MoreLinq;

namespace Scripts.AI
{
    public class Nav2dCell
    {
        private Nav2dNode[,] nodes;
        public Vector3Int gridPos;
        public bool connected = false;

        public Nav2dCell(Vector3Int gridPos)
        {
            this.gridPos = gridPos;
            this.nodes = new Nav2dNode[3, 3];

        }


        /**
        finds the closest accessible Node 
        in the general direction of the enemy
         */

        public Nav2dNode findClosestAccessibleNodeInCell(Vector3 pos)
        {
            return GetNodesList().Where(x => x != null && x.accessible)
            .MinBy(x => (pos - x.worldPos).magnitude)
            .FirstOrDefault();
        }

        public void setNode(Nav2dNode node, Vector2Int direction)
        {
            nodes[direction.x + 1, direction.y + 1] = node;
        }

        public IEnumerable<Nav2dNode> GetNodesList()
        {
            return new List<Nav2dNode> { getNode(0, 0), getNode(-1, -1), getNode(-1, 1), getNode(1, -1), getNode(1, 1) };
        }

        // public Nav2dNode getNode(Vector2Int direction)
        // {
        //     return nodes[direction.x + 1, direction.y + 1];
        // }

        public Nav2dNode getNode(int x, int y)
        {
            lock (nodes)
                return nodes[x + 1, y + 1];
        }
    }
}
