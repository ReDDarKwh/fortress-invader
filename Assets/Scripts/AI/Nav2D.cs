using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using C5;
using UnityEditor;
using System.Threading;
using System.Linq;
using TownGenerator.Wards;

namespace Scripts.AI
{

    public class PathRequest
    {
        public Vector3 Start { get; set; }
        public Vector3 End { get; set; }
        public int AgentGameObjectID { get; set; }
        public Action<IEnumerable<Nav2dNode>, bool> PathCallback { get; set; }
        public float DeterminationLevel { get; internal set; }
    }

    public class Nav2D : MonoBehaviour
    {
        public Grid grid;

        //public Grid optimizationGrid;
        public int width; // in cell
        public int height; // in cell


        public string[] wardLayerMask;

        public bool showNodeConnections;
        public string[] collisionLayers = new string[] { "Buildings" };

        public string[] nav2dNodeLayer = new string[] { "Node" };

        private Dictionary<Vector2Int, Nav2dCell> cellsArray;

        


        [System.NonSerialized]
        public List<Nav2dNode> nodes;

        private int maxRunningTreadNum = 6;

        private Dictionary<int, int> waitingThreadNumberById;
        private Queue<PathRequest> waitingThreads;
        private List<Thread> runningThreads;


        public List<Nav2dNode> GetNodesInCircle(Vector3 point, float radius)
        {
            var centerCell = grid.WorldToCell(point);
            var gridRadius = Mathf.Ceil(radius / grid.cellSize.x);
            var result = new System.Collections.Generic.HashSet<Nav2dNode>();
            for (var y = -gridRadius; y < gridRadius; y++)
            {
                for (var x = -gridRadius; x < gridRadius; x++)
                {
                    var pos = new Vector2Int((int)(centerCell.x + x), (int)(centerCell.y + y));
                    if (cellsArray.ContainsKey(pos))
                    {
                        foreach (var node in cellsArray[pos].GetNodesList()
                        .Where(n => n != null && !result.Contains(n) && ((n.worldPos) - (point)).magnitude < radius))
                        {
                            result.Add(node);
                        };
                    }
                }
            }
            return result.ToList();
        }

        // Use this for initialization
        void Start()
        {
            nodes = new List<Nav2dNode>();
            waitingThreads = new Queue<PathRequest>();
            runningThreads = new List<Thread>();
            waitingThreadNumberById = new Dictionary<int, int>();

            //width = (int)(optimizationGrid.cellSize.x); /// grid.cellSize.x * artiveGridSize);
            //height = (int)(optimizationGrid.cellSize.y); /// grid.cellSize.y * artiveGridSize);

            //transform.position = -new Vector3(width * grid.cellSize.x / 2, height * grid.cellSize.y / 2);

            //transform.position -= new Vector3(width / 2 * grid.cellSize.x, height / 2 * grid.cellSize.y);

        }


        // public void ChangeOffSet(Vector3Int offset)
        // {
        //     if (offset != this.offset)
        //     {
        //         maxCreatePerFrameVec.Set(0, 0);
        //         maxConnectionPerFrameVec.Set(0, 0);
        //         this.offset = offset;
        //         generated = false;
        //     }
        // }

        // Update is called once per frame
        void Update()
        {
            // var middle = optimizationGrid.WorldToCell(playerPos.position) - new Vector3Int(artiveGridSize, artiveGridSize, 0);

            // if (oldMiddle != middle)
            // {
            //     ChangeOffSet(grid.WorldToCell(optimizationGrid.CellToWorld(middle)));
            //     oldMiddle = middle;
            // }

            // var cells = new C5.HashSet<Vector3Int> {
            //     middle,
            // };

            // for (var y = -artiveGridSize; y <= artiveGridSize; y++)
            // {
            //     for (var x = -artiveGridSize; x <= artiveGridSize; x++)
            //     {
            //         cells.Add(middle + new Vector3Int(y, x, 0));
            //     }
            // }

            //Debug.Log("Running threads : " + runningThreads.Count);

            if (showNodeConnections)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        if (cellsArray.ContainsKey(new Vector2Int(x, y)))
                        {

                            if (cellsArray[new Vector2Int(x, y)].getNode(0, 0) != null)
                                cellsArray[new Vector2Int(x, y)].getNode(0, 0).drawConnections();
                            if (cellsArray[new Vector2Int(x, y)].getNode(-1, -1) != null)
                                cellsArray[new Vector2Int(x, y)].getNode(-1, -1).drawConnections();
                        }
                    }
                }
            }


            runningThreads = runningThreads.Where(x => x.IsAlive).ToList();


            // path request logic
            if (runningThreads.Count < maxRunningTreadNum && waitingThreads.Count > 0)
            {

                PathRequest pathRequest;

                do
                {
                    pathRequest = waitingThreads.Dequeue();
                    waitingThreadNumberById[pathRequest.AgentGameObjectID]--;
                } while (
                   waitingThreadNumberById[pathRequest.AgentGameObjectID] != 0
                );

                // remove to save memory
                waitingThreadNumberById.Remove(pathRequest.AgentGameObjectID);

                // create thread
                var startCellPos = grid.LocalToCell(grid.WorldToLocal(pathRequest.Start));
                var endCellPos = grid.LocalToCell(grid.WorldToLocal(pathRequest.End));

                ThreadStart threadStart = delegate
                {

                    pathRequest.PathCallback(Astar.findShortestPath(
                        findClosestAccessibleNodeInGrid(startCellPos, pathRequest.End),
                        findClosestAccessibleNodeInGrid(endCellPos, pathRequest.End),
                        pathRequest.DeterminationLevel
                    ), true);
                };

                var t = new Thread(threadStart);
                runningThreads.Add(t);

                // run the thread
                t.Start();
            }
        }

        void OnDrawGizmos()
        {

            Gizmos.DrawLine(grid.transform.position,
             grid.transform.position + new Vector3(grid.cellSize.x * width, 0, 0));

            Gizmos.DrawLine(grid.transform.position,
             grid.transform.position + new Vector3(0, grid.cellSize.y * height, 0));

            Gizmos.DrawLine(grid.transform.position + new Vector3(grid.cellSize.x * width, grid.cellSize.y * height, 0),
             grid.transform.position + new Vector3(0, grid.cellSize.y * height, 0));

            Gizmos.DrawLine(grid.transform.position + new Vector3(grid.cellSize.x * width, grid.cellSize.y * height, 0),
            grid.transform.position + new Vector3(grid.cellSize.x * width, 0, 0));

        }

        public void GenerateNavMesh(Vector3 origin, float radius)
        {

            cellsArray = new Dictionary<Vector2Int, Nav2dCell>();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {

                    // dont generate cell if outside of city radius;

                    var gridCenter = grid.GetCellCenterWorld(new Vector3Int(x, y, 0));
                    var insideRadius = (gridCenter - origin).magnitude < radius;


                    // ckeck if inside city ward;
                    var insideWard = false;

                    var collider = Physics2D.OverlapPoint(gridCenter, LayerMask.GetMask(wardLayerMask));

                    if (collider != null)
                    {
                        var wardController = collider.GetComponent<WardController>();
                        insideWard = wardController.ward.patch.withinCity;
                    }

                    if (insideRadius && insideWard)
                    {
                        CreateNavCell(new Vector3Int(x, y, 0));
                    }
                }
            }

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var vec = new Vector2Int(x, y);

                    if (cellsArray.ContainsKey(vec))
                    {
                        CreateAdjacentNodesConnections(cellsArray[vec]);
                    }
                }
            }

            // free up memory
            //cellsArray = null;//new Nav2dCell[width, height];
        }


        public bool CanBeReached(Vector3 worldPos)
        {
            var gridpos = grid.WorldToCell(worldPos);
            return (cellsArray[new Vector2Int(gridpos.x, gridpos.y)] != null);
        }

        private void CreateNavCell(Vector3Int gridPos)
        {
            var cellsize = grid.cellSize.x;
            var center = grid.GetCellCenterWorld(gridPos);
            var northWest = center + new Vector3(-cellsize / 2, -cellsize / 2, 0);


            // creating the nodes

            var nodeCenter = new Nav2dNode(center);
            // var nodeObjectCenter = Instantiate(node, nodeCenter.worldPos, Quaternion.identity, this.transform);
            // nodeObjectCenter.node = nodeCenter;

            var nodeNorthWest = new Nav2dNode(northWest);
            //var nodeObjectWest = Instantiate(node, nodeNorthWest.worldPos, Quaternion.identity, this.transform);
            //nodeObjectWest.node = nodeNorthWest;


            //setting nodes in cells

            nodes.Add(nodeCenter);
            nodes.Add(nodeNorthWest);

            var cell = cellsArray[new Vector2Int(gridPos.x, gridPos.y)] = new Nav2dCell(gridPos);
            cell.setNode(nodeCenter, new Vector2Int(0, 0));
            cell.setNode(nodeNorthWest, new Vector2Int(-1, -1));


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


            //return cellsArray[gridPos.x, gridPos.y];
        }


        private void CreateAdjacentNodesConnections(Nav2dCell cell)
        {
            var cells = new Dictionary<string, Nav2dCell>{
                {"leftDown", FindNavCellAtPosition(cell.gridPos + new Vector3Int(-1,-1,0))},
                {"leftTop", FindNavCellAtPosition(cell.gridPos + new Vector3Int(-1,1,0))},   // left down
                {"left", FindNavCellAtPosition(cell.gridPos + new Vector3Int(-1,0,0))},   // left
                {"top", FindNavCellAtPosition(cell.gridPos + new Vector3Int(0,1,0))},   // top
                {"down", FindNavCellAtPosition(cell.gridPos + new Vector3Int(0,-1,0))},   // down
                {"right", FindNavCellAtPosition(cell.gridPos + new Vector3Int(1,0,0))},
                {"rightTop", FindNavCellAtPosition(cell.gridPos + new Vector3Int(1,1,0))},
                {"rightDown", FindNavCellAtPosition(cell.gridPos + new Vector3Int(1,-1,0))}
            };


            // table of nodes to test connection
            Nav2dNode[] centerNodeNeighbors = new Nav2dNode[9]{
                    cell.getNode(-1,-1),
                    cells["left"]?.getNode(0,0),
                    cells["top"]?.getNode(-1,-1),
                    cells["down"]?.getNode(0,0),
                    null,
                    cells["top"]?.getNode(0,0),
                    cells["right"]?.getNode(-1,-1),
                    cells["right"]?.getNode(0,0),
                    cells["rightTop"]?.getNode(-1,-1)
                };

            //sets connection two ways

            cell.getNode(0, 0).setAccessibility(centerNodeNeighbors, LayerMask.GetMask(collisionLayers));

            Nav2dNode[] northWestNodeNeighbors = new Nav2dNode[9]{
                    cells["leftDown"]?.getNode(0,0),
                    cells["left"]?.getNode(-1,-1),
                    cells["left"]?.getNode(0,0),
                    cells["down"]?.getNode(-1,-1),
                    null,
                    cells["top"]?.getNode(-1,-1),
                    cells["down"]?.getNode(0,0),
                    cells["right"]?.getNode(-1,-1),
                    cell.getNode(0,0)
                };

            //sets connection two ways
            cell.getNode(-1, -1).setAccessibility(northWestNodeNeighbors, LayerMask.GetMask(collisionLayers));
        }


        internal void RequestPath(Vector3 start, Vector3 end, int agentGameObjectID, Action<IEnumerable<Nav2dNode>, bool> pathCallback, float determinationLevel)
        {

            // add it to waiting dictionnary
            if (waitingThreadNumberById.ContainsKey(agentGameObjectID))
            {
                waitingThreadNumberById[agentGameObjectID]++;
            }
            else
            {
                waitingThreadNumberById.Add(agentGameObjectID, 1);
            }

            // add it to waiting queue

            waitingThreads.Enqueue(
                new PathRequest
                {
                    Start = start,
                    End = end,
                    AgentGameObjectID = agentGameObjectID,
                    PathCallback = pathCallback,
                    DeterminationLevel = determinationLevel 
                }
            );
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
            if (gridPos.x >= 0 && gridPos.x < width && gridPos.y >= 0 && gridPos.y < height)
            {
                var vec = new Vector2Int(gridPos.x, gridPos.y);
                return cellsArray.ContainsKey(vec) ? cellsArray[vec] : null;
            }

            return null;
        }

    }
}

