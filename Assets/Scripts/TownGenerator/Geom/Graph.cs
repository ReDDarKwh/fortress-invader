using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace TownGenerator.Geom
{

    public class Graph
    {
        public List<Node> nodes = new List<Node>();

        public Graph()
        {
        }

        public Node add(Node node = null)
        {
            if (node == null)
            {
                node = new Node();
            }
            nodes.Add(node);
            return node;
        }

        public void remove(Node node)
        {
            node.unlinkAll();
            nodes.Remove(node);
        }

        public List<Node> aStar(Node start, Node goal, List<Node> exclude = null)
        {
            var closedSet = exclude != null ? new List<Node>(exclude) : new List<Node>();
            var openSet = new List<Node> { start };
            var cameFrom = new Dictionary<Node, Node>();

            var gScore = new Dictionary<Node, float>() { { start, 0f } };

            while (openSet.Count > 0)
            {
                var current = openSet[0];
                openSet.RemoveAt(0);

                if (current == goal)
                    return buildPath(cameFrom, current);

                openSet.Remove(current);
                closedSet.Add(current);

                var curScore = gScore[current];
                foreach (var neighbour in current.links.Keys)
                {
                    if (closedSet.Contains(neighbour))
                        continue;

                    var score = curScore + current.links[neighbour];
                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                    else if (score >= gScore[neighbour])
                        continue;

                    cameFrom[neighbour] = current;
                    gScore[neighbour] = score;
                }
            }

            return null;
        }

        private List<Node> buildPath(Dictionary<Node, Node> cameFrom, Node current)
        {
            var path = new List<Node> { current };

            while (cameFrom.ContainsKey(current))
                path.Add(current = cameFrom[current]);

            return path;
        }

        // public float? calculatePrice(List<Node> path)
        // {
        //     if (path.Count < 2)
        //     {
        //         return 0;
        //     }

        //     var price = 0.0;
        //     var current = path[0];
        //     var next = path[1];
        //     for (var i = 0; i < path.Count - 1; i++)
        //     {
        //         if (current.links.ContainsKey(next))
        //         {
        //             price += current.links[next];
        //         }
        //         else
        //         {
        //             return null;
        //         }
        //         current = next;
        //         next = path[i + 1];
        //     }
        //     return (float)price;
        // }
    }

    public class Node
    {
        public Dictionary<Node, float> links = new Dictionary<Node, float>();

        public Node() { }

        public void link(Node node, float price = 1, bool symmetrical = true)
        {
            links[node] = price;
            if (symmetrical)
            {
                node.links[this] = price;
            }
        }

        public void unlink(Node node, bool symmetrical = true)
        {
            links.Remove(node);
            if (symmetrical)
            {
                node.links.Remove(this);
            }
        }

        public void unlinkAll()
        {
            foreach (var node in links.Keys)
            {
                unlink(node);
            }
        }
    }
}