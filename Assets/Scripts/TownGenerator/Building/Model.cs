using System;
using System.Collections.Generic;
using TownGenerator.Geom;
using TownGenerator.Wards;
using UnityEngine;
using System.Linq;
using MoreLinq;

using Street = TownGenerator.Geom.Polygon;
using Random = UnityEngine.Random;

namespace TownGenerator.Building
{
    public class Model
    {

        public static Model instance;

        // Small Town	6
        // Large Town	10
        // Small City	15
        // Large City	24
        // Metropolis	40
        private int nPatches;

        private bool plazaNeeded;
        private bool citadelNeeded;
        private bool wallsNeeded;

        public static List<Type> WARDS = new List<Type> {
          typeof(CraftsmenWard),  typeof(CraftsmenWard),  typeof(MerchantWard),  typeof(CraftsmenWard),  typeof(CraftsmenWard),  typeof(Cathedral),
         typeof(CraftsmenWard),  typeof(CraftsmenWard),  typeof(CraftsmenWard),  typeof(CraftsmenWard),  typeof(CraftsmenWard),
        typeof(CraftsmenWard), typeof(CraftsmenWard),  typeof(CraftsmenWard),  typeof(AdministrationWard), typeof(CraftsmenWard),
         typeof(Slum),  typeof(CraftsmenWard),  typeof(Slum),  typeof(PatriciateWard),  typeof(Market),
         typeof(Slum),  typeof(CraftsmenWard),  typeof(CraftsmenWard),  typeof(CraftsmenWard),  typeof(Slum),
         typeof(CraftsmenWard),  typeof(CraftsmenWard),  typeof(CraftsmenWard),  typeof(MilitaryWard),  typeof(Slum),
         typeof(CraftsmenWard),  typeof(Park),  typeof(PatriciateWard),  typeof(Market),  typeof(MerchantWard)
        };

        public Topology topology;

        public List<Patch> patches;
        public List<Patch> waterbody;
        // For a walled city it's a list of patches within the walls,
        // for a city without walls it's just a list of all city wards
        public List<Patch> inner;
        public Patch citadel;
        public Patch plaza;
        public Point center;

        public CurtainWall border;
        public CurtainWall wall;

        public float cityRadius;

        // List of all entrances of a city including castle gates
        public List<Point> gates;

        // Joined list of streets (inside walls) and roads (outside walls)
        // without diplicating segments
        public List<Street> arteries;
        public List<Street> streets;
        public List<Street> roads;



        public Model(CitySettings settings)
        {
            this.nPatches = settings.patchNum != -1 ? settings.patchNum : 15;

            plazaNeeded = true;
            citadelNeeded = true;
            wallsNeeded = true;

            var seed = settings.seed;

            do
            {
                try
                {
                    Random.InitState(++seed);
                    Debug.Log(seed);

                    build();
                    instance = this;
                }
                catch (Exception e)
                {
                    Debug.Log(
                        e.StackTrace
                    );

                    instance = null;
                }
                finally
                {
                    Random.State oldstate = Random.state;
                    Random.state = oldstate;
                }
            }
            while (instance == null);

        }


        private void build()
        {
            streets = new List<Polygon>();
            roads = new List<Polygon>();

            buildPatches();
            optimizeJunctions();
            buildWalls();
            buildStreets();
            createWards();
            buildGeometry();
        }

        private void buildPatches()
        {
            var sa = Random.value * 2 * Math.PI;
            var points = new List<Point>();

            for (var i = 0; i < nPatches * 8; i++)
            {
                var a = sa + Mathf.Sqrt(i) * 5;
                var r = (i == 0 ? 0 : (10 + i * (2 + Random.value)));
                points.Add(new Point(Mathf.Cos((float)a) * r, Mathf.Sin((float)a) * r));
            }
            var voronoi = Voronoi.build(points);

            // Relaxing central wards
            for (var i = 0; i < 3; i++)
            {
                var toRelax = new List<Point>();
                for (var j = 0; j < 3; j++)
                {
                    toRelax.Add(voronoi.points[j]);
                };
                toRelax.Add(voronoi.points[nPatches]);
                voronoi = Voronoi.relax(voronoi, toRelax);
            }

            voronoi.points.Sort((p1, p2) =>
            {

                return (int)Mathf.Round(Mathf.Sign(p1.vec.magnitude - p2.vec.magnitude));
            });
            var regions = voronoi.partioning();

            patches = new List<Patch>();
            inner = new List<Patch>();

            var count = 0;
            foreach (var r in regions)
            {
                var patch = Patch.fromRegion(r);
                patches.Add(patch);

                if (count == 0)
                {
                    center =
                    patch.shape.MinBy((Point p) => { return (p.vec).magnitude; }).FirstOrDefault();


                    if (plazaNeeded)
                        plaza = patch;
                }
                else if (count == nPatches && citadelNeeded)
                {
                    citadel = patch;
                    citadel.withinCity = true;
                }

                if (count < nPatches)
                {
                    patch.withinCity = true;
                    patch.withinWalls = wallsNeeded;
                    inner.Add(patch);
                }

                count++;
            }
        }

        private void buildWalls()
        {
            var reserved = citadel != null ? citadel.shape : new List<Point>();

            border = new CurtainWall(wallsNeeded, this, inner, reserved);
            if (wallsNeeded)
            {
                wall = border;
                wall.buildTowers();
            }

            var radius = border.getRadius();
            patches = patches.Where((Patch p) => { return p.shape.distance(center) < radius * 3; }).ToList();

            if (patches.Count <= 0)
            {

                Debug.Log("all pasfsrg");
            }

            gates = border.gates;

            if (citadel != null)
            {
                var castle = new Castle(this, citadel);
                castle.wall.buildTowers();
                citadel.ward = castle;

                if (citadel.shape.compactness < 0.75)
                    throw new GenerationFailedException("Bad citadel shape!");

                gates = gates.Concat(castle.wall.gates).ToList();
            }
        }

        public static Polygon findCircumference(List<Patch> wards)
        {
            if (wards.Count == 0)
            {

                return new Polygon();
            }
            else
            {
                if (wards.Count == 1)
                    return new Polygon(wards[0].shape);
            }


            var edges = new List<Tuple<Point, Point>>();

            foreach (var w1 in wards)
            {
                w1.shape.forEdge((a, b) =>
                {
                    var outerEdge = true;


                    foreach (var w2 in wards)
                    {
                        if (w2.shape.findEdge(b, a) != -1)
                        {
                            outerEdge = false;
                            break;
                        }
                    }

                    if (outerEdge)
                    {


                        edges.Add(new Tuple<Point, Point>(a, b));
                    }
                });
            }

            var result = new Polygon(
                findNextEdge(edges[0], edges[0], edges, new List<Tuple<Point, Point>> { edges[0] }).Select(x => x.Item1)
                .ToList()
            );
            // var index = 0;
            // var count = 0;


            // do
            // {
            //     count++;
            //     result.Add(A[index]);
            //     index = A.IndexOf(B[index]);

            //     // max number of tries

            // }
            // while (index != 0 && count < 50000);

            for (var i = 0; i < result.Count; i++)
            {
                Debug.DrawLine(result[i].vec, result[(i + 1) % result.Count].vec, Color.red, 20 + i);
            }

            return result;
        }

        private static List<Tuple<Point, Point>> findNextEdge(Tuple<Point, Point> start, Tuple<Point, Point> edge, List<Tuple<Point, Point>> edges, List<Tuple<Point, Point>> result)
        {
            var index = edges.FindIndex(x => x.Item1 == edge.Item2);
            var found = edges[index];

            result.Add(found);
            edges.RemoveAt(index);

            if (start.Item1 != found.Item2)
            {
                return findNextEdge(start, found, edges, result);
            }
            else
            {
                return result;
            }
        }

        public List<Patch> patchByVertex(Point v)
        {
            return patches.Where(
                (patch) => { return patch.shape.Contains(v); }
        ).ToList();
        }


        private void smoothStreet(Street street)
        {
            var smoothed = street.smoothVertexEq(3);
            for (var i = 1; i < street.Count - 1; i++)
                street[i] = smoothed[i];
        }

        private void buildStreets()
        {

            topology = new Topology(this);

            foreach (var gate in gates)
            {
                // Each gate is connected to the nearest corner of the plaza or to the central junction
                var end = plaza != null ?
                    plaza.shape.MinBy((v) => { return (v.vec - gate.vec).magnitude; }).FirstOrDefault() : center;

                var street = topology.buildPath(gate, end, topology.outer);
                if (street != null)
                {
                    streets.Add(new Street(street));

                    if (border.gates.Contains(gate))
                    {
                        var dir = gate.vec.normalized * (1000);
                        Point start = null;
                        var dist = Mathf.Infinity;
                        foreach (var p in topology.node2pt.Values)
                        {
                            var d = (p.vec - dir).magnitude;
                            if (d < dist)
                            {
                                dist = d;
                                start = p;
                            }
                        }

                        var road = topology.buildPath(start, gate, topology.inner);
                        if (road != null)
                            roads.Add(new Street(road));
                    }
                }
                else
                {

                    throw new GenerationFailedException("Unable to build a street!");
                }
            }

            tidyUpRoads();

            foreach (var a in arteries)
                smoothStreet(a);
        }



        private void cut2segments(Street street, List<Segment> segments)
        {
            Point v0;
            var v1 = street[0];
            for (var i = 1; i < streets.Count; i++)
            {
                v0 = v1;
                v1 = street[i];

                // Removing segments which go along the plaza
                if (plaza != null && plaza.shape.Contains(v0) && plaza.shape.Contains(v1))
                    continue;

                var exists = false;
                foreach (var seg in segments)
                    if (seg.start == v0 && seg.end == v1)
                    {
                        exists = true;
                        break;
                    }

                if (!exists)
                    segments.Add(new Segment(v0, v1));
            }
        }

        private void tidyUpRoads()
        {
            var segments = new List<Segment>();


            foreach (var street in streets)
                cut2segments(street, segments);
            foreach (var road in roads)
                cut2segments(road, segments);

            arteries = new List<Street>();
            while (segments.Count > 0)
            {
                var seg = segments[segments.Count - 1];
                segments.RemoveAt(segments.Count - 1);

                var attached = false;
                foreach (var a in arteries)
                {
                    if (a[0] == seg.end)
                    {
                        a.Insert(0, seg.start);
                        attached = true;
                        break;
                    }
                    else if (a[a.Count - 1] == seg.start)
                    {
                        a.Add(seg.end);
                        attached = true;
                        break;
                    }
                }

                if (!attached)
                    arteries.Add(new Street(new List<Point> { seg.start, seg.end }));
            }
        }

        private void optimizeJunctions()
        {

            var patchesToOptimize =
                citadel == null ? inner : inner.Concat(new List<Patch> { citadel });

            var wards2clean = new List<Patch>();
            foreach (var w in patchesToOptimize)
            {
                var index = 0;
                while (index < w.shape.Count)
                {

                    var v0 = w.shape[index];
                    var v1 = w.shape[(index + 1) % w.shape.Count];

                    if (v0 != v1 && (v0.vec - v1.vec).magnitude < 8)
                    {
                        foreach (var w1 in patchByVertex(v1))
                        {
                            if (w1 != w)
                            {
                                w1.shape[w1.shape.IndexOf(v1)] = v0;
                                wards2clean.Add(w1);
                            }
                        }
                        v0.vec += (v1.vec);
                        v0.vec *= (0.5f);
                        w.shape.Remove(v1);
                    }
                    index++;
                }
            }

            // Removing duplicate vertices
            foreach (var w in wards2clean)
            {
                for (var i = 0; i < w.shape.Count; i++)
                {
                    var v = w.shape[i];
                    int dupIdx;
                    while ((dupIdx = w.shape.IndexOf(v, i + 1)) != -1)
                    {
                        w.shape.RemoveRange(dupIdx, 1);
                    }
                }
            }
        }

        private void createWards()
        {
            var unassigned = new List<Patch>(inner);
            if (plaza != null)
            {
                plaza.ward = new Market(this, plaza);
                unassigned.Remove(plaza);
            }

            // Assigning inner city gate wards
            foreach (var gate in border.gates)
            {
                foreach (var patch in patchByVertex(gate))
                {

                    if (patch.withinCity && patch.ward == null && Random.value < (wall == null ? 0.2 : 0.5))
                    {
                        patch.ward = new GateWard(this, patch);
                        unassigned.Remove(patch);
                    }

                }
            }

            var wards = new List<Type>(WARDS);
            // some shuffling
            for (var i = 0; i < (int)(wards.Count / 10); i++)
            {
                var index = Random.Range(0, (wards.Count - 1));
                var tmp = wards[index];
                wards[index] = wards[index + 1];
                wards[index + 1] = tmp;
            }

            // Assigning inner city wards
            while (unassigned.Count > 0)
            {
                Patch bestPatch = null;

                var wardClass = wards.Count > 0 ? wards[0] : typeof(Slum);

                if (wards.Count > 0)
                    wards.RemoveAt(0);


                var rateFunc = wardClass.GetMethod("rateLocation");


                if (rateFunc == null)
                {
                    do
                    {
                        bestPatch = unassigned[Random.Range(0, unassigned.Count - 1)];
                    }
                    while (bestPatch.ward != null);
                }
                else
                {
                    bestPatch = unassigned.MinBy((Patch patch) =>
                    {
                        return patch.ward == null ?
                        rateFunc.Invoke(null, new object[] { this, patch })
                       : Mathf.Infinity;
                    }).FirstOrDefault();
                }

                bestPatch.ward = (Ward)Activator.CreateInstance(wardClass, new object[] { this, bestPatch });

                unassigned.Remove(bestPatch);
            }

            // Outskirts
            if (wall != null)
                foreach (var gate in wall.gates)
                {
                    if (!(Random.value < (1 / (nPatches - 5))))
                    {
                        foreach (var patch in patchByVertex(gate))
                        {
                            if (patch.ward == null)
                            {
                                patch.withinCity = true;
                                patch.ward = new GateWard(this, patch);
                            }
                        }
                    }
                }

            // Calculating radius and processing countryside
            cityRadius = 0;
            foreach (var patch in patches)
            {

                if (patch.withinCity)
                {
                    // Radius of the city is the farthest point of all wards from the center
                    foreach (var v in patch.shape)
                        cityRadius = Mathf.Max(cityRadius, v.vec.magnitude);

                }
                else
                {
                    if (patch.ward == null)
                        patch.ward = Random.value < (0.2) && patch.shape.compactness >= 0.7 ?
                            new Farm(this, patch) :

                            new Ward(this, patch);
                }
            }
        }

        private void buildGeometry()
        {
            foreach (var patch in patches)
                patch.ward.createGeometry();
        }


        public Patch getNeighbour(Patch patch, Point v)
        {
            var next = patch.shape.next(v);
            foreach (var p in patches)
                if (p.shape.findEdge(next, v) != -1)
                    return p;
            return null;
        }

        public List<Patch> getNeighbours(Patch patch)
        {
            return patches.Where((Patch p) => { return p != patch && p.shape.borders(patch.shape); }).ToList();
        }

        // A ward is "enclosed" if it belongs to the city and
        // it's surrounded by city wards and water
        public bool isEnclosed(Patch patch)
        {
            return patch.withinCity && (patch.withinWalls || getNeighbours(patch).All((Patch p) => { return p.withinCity; }));
        }


    }
}