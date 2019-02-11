using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using C5;
using UnityEditor;
using System.Threading;
using System.Linq;

namespace Scripts.AI
{

    public class Nav2D : MonoBehaviour
    {
        public Grid grid;
        public int width; // in cell
        public int height; // in cell
        public bool showNodeConnections;
        public string[] collisionLayers = new string[] { "Buildings" };

        public NodeController node;

        public string[] nav2dNodeLayer = new string[] { "Node" };
        private Nav2dCell[,] cellsArray;
        private List<Nav2dNode> debugPath;
        private List<Nav2dNode> nodes;



        public List<Nav2dNode> GetNodesInCircle(Vector3 point, float radius) {

            return Physics2D.OverlapCircleAll(point, radius, 
            LayerMask.GetMask(nav2dNodeLayer)
                ).Select(
            x=>x.GetComponent<NodeController>().node
                ).ToList();
        
        }


        // Use this for initialization
        void Start()
        {
            nodes = new List<Nav2dNode>();
            GenerateNavMesh();
        }
        // Update is called once per frame
        void Update()
        {
            if (showNodeConnections)
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        cellsArray[x, y].getNode(0, 0).drawConnections();
                        cellsArray[x, y].getNode(-1, -1).drawConnections();
                    }
                }
        }
        void OnDrawGizmos()
        {
            if (debugPath != null)
            {

                var path = debugPath.ToArray();
                if (path.Length < 2) return;

                for (var i = 0; i < path.Length - 1; i++)
                {

                    var current = path[i];
                    var next = path[i + 1];

                    Gizmos.color = new Color(0.2f, 1, 1);
                    Gizmos.DrawLine(current.worldPos, next.worldPos);

                }
            }

            Gizmos.DrawLine(grid.transform.position,
             grid.transform.position + new Vector3(grid.cellSize.x * width, 0, 0));

            Gizmos.DrawLine(grid.transform.position,
             grid.transform.position + new Vector3(0, grid.cellSize.y * height, 0));

            Gizmos.DrawLine(grid.transform.position + new Vector3(grid.cellSize.x * width, grid.cellSize.y * height, 0),
             grid.transform.position + new Vector3(0, grid.cellSize.y * height, 0));

            Gizmos.DrawLine(grid.transform.position + new Vector3(grid.cellSize.x * width, grid.cellSize.y * height, 0),
                grid.transform.position + new Vector3(grid.cellSize.x * width, 0, 0));

        }






        public void GenerateNavMesh()
        {
            cellsArray = new Nav2dCell[width, height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    CreateNavCell(new Vector3Int(x, y, 0));
                }
            }
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    CreateAdjacentNodesConnections(cellsArray[x, y]);
                }
            }
        }
        private Nav2dCell CreateNavCell(Vector3Int gridPos)
        {
            var cellsize = grid.cellSize.x;
            var center = grid.GetCellCenterWorld(gridPos);
            var northWest = center + new Vector3(-cellsize / 2, -cellsize / 2, 0);



            // creating the nodes

            var nodeCenter = new Nav2dNode(center);
            var nodeObjectCenter = Instantiate(node, nodeCenter.worldPos, Quaternion.identity);
            nodeObjectCenter.node = nodeCenter;

            var nodeNorthWest = new Nav2dNode(northWest);
            var nodeObjectWest = Instantiate(node, nodeNorthWest.worldPos, Quaternion.identity);
            nodeObjectWest.node = nodeNorthWest;



            //setting nodes in cells

            nodes.Add(nodeCenter);
            nodes.Add(nodeNorthWest);

            cellsArray[gridPos.x, gridPos.y] = new Nav2dCell(gridPos);
            cellsArray[gridPos.x, gridPos.y].setNode(nodeCenter, new Vector2Int(0, 0));
            cellsArray[gridPos.x, gridPos.y].setNode(nodeNorthWest, new Vector2Int(-1, -1));

            var leftCell = FindNavCellAtPosition(gridPos + new Vector3Int(-1, 0, 0));
            if (leftCell != null)
            {
                leftCell.setNode(nodeNorthWest, new Vector2Int(1, -1));
            }

            var downCell = FindNavCellAtPosition(gridPos + new Vector3Int(0, -1, 0));
            if (downCell != null)
            {
                downCell.setNode(nodeNorthWest, new Vector2Int(-1, 1));
            }

            var leftDownCell = FindNavCellAtPosition(gridPos + new Vector3Int(-1, -1, 0));
            if (leftDownCell != null)
            {
                leftDownCell.setNode(nodeNorthWest, new Vector2Int(1, 1));
            }

            return cellsArray[gridPos.x, gridPos.y];
        }
        private void CreateAdjacentNodesConnections(Nav2dCell cell)
        {
            // table of nodes to test connection
            Nav2dNode[] centerNodeNeighbors = new Nav2dNode[9]{

                    cell.getNode(-1,-1),
                    FindNavCellAtPosition(cell.gridPos + new Vector3Int(-1,0,0))?.getNode(0,0),
                    FindNavCellAtPosition(cell.gridPos + new Vector3Int(0,1,0))?.getNode(-1,-1),

                    FindNavCellAtPosition(cell.gridPos + new Vector3Int(0,-1,0))?.getNode(0,0),
                    null,
                    FindNavCellAtPosition(cell.gridPos + new Vector3Int(0,1,0))?.getNode(0,0),

                    FindNavCellAtPosition(cell.gridPos + new Vector3Int(1,0,0))?.getNode(-1,-1),
                    FindNavCellAtPosition(cell.gridPos + new Vector3Int(1,0,0))?.getNode(0,0),
                    FindNavCellAtPosition(cell.gridPos + new Vector3Int(1,1,0))?.getNode(-1,-1)
            };

            cell.getNode(0, 0).setAccessibility(centerNodeNeighbors, LayerMask.GetMask(collisionLayers));

            Nav2dNode[] northWestNodeNeighbors = new Nav2dNode[9]{

                    FindNavCellAtPosition(cell.gridPos + new Vector3Int(-1,-1,0))?.getNode(0,0),
                    FindNavCellAtPosition(cell.gridPos + new Vector3Int(-1,0,0))?.getNode(-1,-1),
                    FindNavCellAtPosition(cell.gridPos + new Vector3Int(-1,0,0))?.getNode(0,0),

                    FindNavCellAtPosition(cell.gridPos + new Vector3Int(0,-1,0))?.getNode(-1,-1),
                    null,
                    FindNavCellAtPosition(cell.gridPos + new Vector3Int(0,1,0))?.getNode(-1,-1),

                    FindNavCellAtPosition(cell.gridPos + new Vector3Int(0,-1,0))?.getNode(0,0),
                    FindNavCellAtPosition(cell.gridPos + new Vector3Int(1, 0 ,0))?.getNode(-1,-1),
                    cell.getNode(0,0)
            };

            cell.getNode(-1, -1).setAccessibility(northWestNodeNeighbors, LayerMask.GetMask(collisionLayers));
        }
        // public IEnumerable<Nav2dNode> GetPath(Vector3 start, Vector3 goal)
        // {
        //     var path = Astar.findShortestPath(
        //         findClosestAccessibleNodeInGrid(start, goal),
        //        findClosestAccessibleNodeInGrid(goal, goal)
        //    );


        //     return path;
        // }

        internal void RequestPath(Vector3 start, Vector3 end, Action<IEnumerable<Nav2dNode>> pathCallback)
        {

            var startCellPos = grid.LocalToCell(grid.WorldToLocal(start));
            var endCellPos = grid.LocalToCell(grid.WorldToLocal(end));

            ThreadStart threadStart = delegate
            {
                pathCallback(Astar.findShortestPath(
                    findClosestAccessibleNodeInGrid(startCellPos, end),
                    findClosestAccessibleNodeInGrid(endCellPos, end)
                ));
            };

            new Thread(threadStart).Start();
        }


        public Nav2dNode findClosestAccessibleNodeInGrid(Vector3Int cellPos, Vector3 targetPos)
        {

            Nav2dNode node = null;
            C5.HashSet<Vector3Int> explored = new C5.HashSet<Vector3Int>();
            Queue<Vector3Int> opened = new Queue<Vector3Int>();

            opened.Enqueue(cellPos);

            // dont try to long
            for (int i = 0; i < 20; i++)
            {
                Vector3Int current = opened.Dequeue();
                Nav2dCell cell = FindNavCellAtPosition(current);

                node = cell?.findClosestAccessibleNodeInCell(targetPos);

                if (node != null)
                    break;

                explored.Add(current);

                Vector3Int[] neighbors = {
                    current + new Vector3Int(0, 1, 0),
                    current + new Vector3Int(0, -1, 0),
                    current + new Vector3Int(-1, 0, 0),
                    current + new Vector3Int(1, 0, 0)
                };

                foreach (var n in neighbors)
                {
                    if (!explored.Contains(n))
                    {
                        opened.Enqueue(n);
                    }
                }
            }
            return node;
        }


        private Nav2dCell FindNavCellAtPosition(Vector3Int gridPos)
        {
            // outside grid bounds
            if (gridPos.x < 0 || gridPos.x >= width || gridPos.y < 0 || gridPos.y >= height)
            {
                return null;
            }
            else
            {
                lock (cellsArray)
                {
                    if (cellsArray[gridPos.x, gridPos.y] == null)
                    {
                        return null;
                    }
                    else
                    {
                        return cellsArray[gridPos.x, gridPos.y];
                    }
                }
            }
        }
        // private Nav2dCell getNavCellAtWorldPos(Vector3 pos)
        // {

        //     var gridPos = grid.LocalToCell(grid.WorldToLocal(pos));

        //     if (gridPos.x < 0 || gridPos.x >= width || gridPos.y < 0 || gridPos.y >= height)
        //     {
        //         return null;
        //     }

        //     return cellsArray[gridPos.x, gridPos.y];
        // }

    }
}

