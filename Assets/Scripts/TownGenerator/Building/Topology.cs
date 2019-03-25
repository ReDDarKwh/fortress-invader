using System.Collections;
using System.Collections.Generic;
using TownGenerator.Geom;
using TownGenerator.Wards;
using UnityEngine;
using System.Linq;

namespace TownGenerator.Building
{
    public class Topology
    {


        private Model model;

        private Graph graph;

        public Dictionary<Point, Node> pt2node;
        public Dictionary<Node, Point> node2pt;

        private List<Point> blocked;

        public List<Node> inner;
        public List<Node> outer;

        public Topology(Model model)
        {
            this.model = model;

            graph = new Graph();
            pt2node = new Dictionary<Point, Node>();
            node2pt = new Dictionary<Node, Point>();

            inner = new List<Node>();
            outer = new List<Node>();

            // Building a list of all blocked points (shore + walls excluding gates)
            blocked = new List<Point>();
            if (model.citadel != null)
                blocked = blocked.Concat(model.citadel.shape).ToList();
            if (model.wall != null)
                blocked = blocked.Concat(model.wall.shape).ToList();
            blocked = blocked.Where(x => !model.gates.Select(g => g.vec).ToList().Contains(x.vec)).ToList();

            var border = model.border.shape;

            foreach (var p in model.patches)
            {
                var withinCity = p.withinCity;

                var v1 = p.shape[p.shape.Count - 1];
                var n1 = processPoint(v1);

                for (var i = 0; i < p.shape.Count; i++)
                {
                    var v0 = v1; v1 = p.shape[i];
                    var n0 = n1; n1 = processPoint(v1);

                    if (n0 != null && !border.contains(v0))
                    {
                        if (withinCity)
                            inner.Add(n0);
                    }
                    else
                    {
                        outer.Add(n0);
                    }

                    if (n1 != null && !border.contains(v1))
                    {
                        if (withinCity)
                            inner.Add(n1);
                    }
                    else
                    {
                        outer.Add(n1);
                    }

                    if (n0 != null && n1 != null)
                        n0.link(n1, (v0.vec - v1.vec).magnitude);
                }
            }
        }

        private Node processPoint(Point v)
        {
            Node n;

            if (pt2node.ContainsKey(v))
            {
                n = pt2node[v];
            }
            else
            {
                pt2node[v] = n = graph.add();
                node2pt[n] = v;
            }

            return blocked.Contains(v) ? null : n;
        }

        public List<Point> buildPath(Point from, Point to, List<Node> exclude = null)
        {
            var path = graph.aStar(pt2node[from], pt2node[to], exclude);
            return path == null ? null : path.Select(n => node2pt[n]).ToList();
        }

    }
}