


using System;
using System.Collections.Generic;
using TownGenerator.Building;

using TownGenerator.Geom;
using System.Linq;
using UnityEngine;

using MoreLinq;
using Random = UnityEngine.Random;

namespace TownGenerator.Wards
{

    public class Ward
    {
        public static float MAIN_STREET = 5.0f;
        public static float REGULAR_STREET = 2.0f;
        public static float ALLEY = 0.7f;

        public Model model;
        public Patch patch;

        public List<Polygon> geometry;


        public Ward(Model model, Patch patch)
        {
            this.model = model;
            this.patch = patch;
        }

        public virtual void createGeometry()
        {
            geometry = new List<Polygon>();

        }

        public Polygon getCityBlock()
        {
            var insetDist = new List<float>();

            var innerPatch = model.wall == null || patch.withinWalls;
            patch.shape.forEdge((v0, v1) =>
            {
                if (model.wall != null && model.wall.bordersBy(patch, v0, v1))
                    // Not too close to the wall
                    insetDist.Add(MAIN_STREET / 2);
                else
                {
                    var onStreet = innerPatch && (model.plaza != null && model.plaza.shape.findEdge(v1, v0) != -1);
                    if (!onStreet)
                        foreach (var street in model.arteries)
                            if (street.contains(v0) && street.contains(v1))
                            {
                                onStreet = true;
                                break;
                            }
                    insetDist.Add((onStreet ? MAIN_STREET : (innerPatch ? REGULAR_STREET : ALLEY)) / 2);
                }
            });

            return patch.shape.isConvex() ?
                patch.shape.shrink(insetDist) :
                patch.shape.buffer(insetDist);
        }



        private void addEdge(Point v1, Point v2, List<PupulatedEdge> populatedEdges, float factor = 1)
        {
            var dx = v2.x - v1.x;
            var dy = v2.y - v1.y;
            var distances = new Dictionary<Point, float>();
            var dd = patch.shape.MaxBy((Point v) =>
            {
                distances[v] = (v != v1
                 && v != v2 ? GeomUtils.distance2line(v1.x, v1.y, dx, dy, v.x, v.y) : 0) * factor;
                return distances[v];
            }).FirstOrDefault();

            populatedEdges.Add(new PupulatedEdge
            {
                x = v1.x,
                y = v1.y,
                dx = dx,
                dy = dy,
                d = distances[dd]
            });
        }

        private class PupulatedEdge
        {
            public float x;
            public float y;
            public float dx;
            public float dy;
            public float d;
        }

        protected void filterOutskirts()
        {
            var populatedEdges = new List<PupulatedEdge>();

            patch.shape.forEdge((Point v1, Point v2) =>
            {
                var onRoad = false;
                foreach (var street in model.arteries)
                    if (street.contains(v1) && street.contains(v2))
                    {
                        onRoad = true;
                        break;
                    }

                if (onRoad)
                    addEdge(v1, v2, populatedEdges, 1);
                else
                {
                    var n = model.getNeighbour(patch, v1);
                    if (n != null)
                        if (n.withinCity)
                            addEdge(v1, v2, populatedEdges, model.isEnclosed(n) ? 1.0f : 0.4f);
                }
            });

            // For every vertex: if this belongs only
            // to patches within city, then 1, otherwise 0
            var density = patch.shape.Select(v =>
            {

                if (model.gates.Contains(v))
                {

                    return 1;
                }
                else
                {

                    return model.patchByVertex(v).All((Patch p) => { return p.withinCity; }) ? 2 * Random.value : 0;
                }

            }).ToList();



            geometry = geometry.Where((Polygon building) =>
            {
                var minDist = 1.0;
                foreach (var edge in populatedEdges)
                    foreach (var v in building)
                    {
                        // Distance from the center of the building to the edge
                        var d = GeomUtils.distance2line(edge.x, edge.y, edge.dx, edge.dy, v.x, v.y);
                        var dist = d / edge.d;
                        if (dist < minDist)
                            minDist = dist;
                    }

                var c = building.center;
                var i = patch.shape.interpolate(c);
                var p = 0.0;
                for (var j = 0; j < i.Count; j++)
                    p += density[j] * i[j];
                minDist /= p;

                return Random.value > minDist;
            }).ToList();
        }

        public virtual string getLabel()
        {
            return null;
        }

        public static float rateLocation(Model model, Patch patch)
        {


            return 0;
        }

        public static List<Polygon> createAlleys(Polygon p, float minSq, float gridChaos, float sizeChaos, float emptyProb = 0.04f, bool split = true)
        {
            // Looking for the longest edge to cut it
            Point v = null;
            var length = -1.0;
            p.forEdge((p0, p1) =>
            {
                var len = (p0.vec - p1.vec).magnitude;
                if (len > length)
                {
                    length = len;
                    v = p0;
                }
            });

            var spread = 0.8 * gridChaos;
            var ratio = (1 - spread) / 2 + Random.value * spread;

            // Trying to keep buildings rectangular even in chaotic wards
            var angleSpread = Mathf.PI / 6 * gridChaos * (p.square < minSq * 4 ? 0.0 : 1);
            var b = (Random.value - 0.5) * angleSpread;

            var halves = Cutter.bisect(p, v, (float)ratio, (float)b, split ? ALLEY : 0.0f);

            var buildings = new List<Polygon>();
            foreach (var half in halves)
            {
                if (half.square < minSq * Mathf.Pow(2, (float)(4 * sizeChaos * (Random.value - 0.5))))
                {
                    if (Random.value > emptyProb)
                        buildings.Add(half);
                }
                else
                {
                    buildings = buildings.Concat(createAlleys(half, minSq, gridChaos, sizeChaos, emptyProb, half.square > minSq / (Random.value * Random
                         .value))).ToList();
                }
            }

            return buildings;
        }

        private static Point findLongestEdge(Polygon poly)
        {
            return poly.MinBy((v) => { return -poly.vector(v).vec.magnitude; }).FirstOrDefault();
        }

        private static List<Polygon> slice(Polygon poly, Point c1, Point c2, float minBlockSq, float fill)
        {
            var v0 = findLongestEdge(poly);
            var v1 = poly.next(v0);
            var v = v1.vec - (v0.vec);

            var ratio = 0.4f + Random.value * 0.2f;
            var p1 = GeomUtils.interpolate(v0, v1, ratio);

            Point c;

            if (Mathf.Abs(GeomUtils.scalar(v.x, v.y, c1.x, c1.y)) < Mathf.Abs(GeomUtils.scalar(v.x, v.y, c2.x, c2.y)))
            {
                c = c1;
            }
            else
            {
                c = c2;
            };

            var halves = poly.cut(p1, new Point(p1.vec + (c.vec)));
            var buildings = new List<Polygon>();
            foreach (var half in halves)
            {
                if (half.square < minBlockSq * Mathf.Pow(2, Random.value * 2 - 1))
                {
                    if (Random.value < (fill))
                        buildings.Add(half);
                }
                else
                {
                    buildings = buildings.Concat(slice(half, c1, c2, minBlockSq, fill)).ToList();
                }
            }
            return buildings;
        }

        public static List<Polygon> createOrthoBuilding(Polygon poly, float minBlockSq, float fill)
        {

            if (poly.square < minBlockSq)
            {
                return new List<Polygon> { poly };
            }
            else
            {
                var c1 = poly.vector(findLongestEdge(poly));
                var c2 = Quaternion.Euler(0, 0, 90) * c1.vec;
                while (true)
                {
                    var blocks = slice(poly, c1, new Point(c2), minBlockSq, fill);
                    if (blocks.Count > 0)
                        return blocks;
                }
            }
        }


    }

}