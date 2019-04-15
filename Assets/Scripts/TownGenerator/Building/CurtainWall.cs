using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TownGenerator.Geom;
using MoreLinq;
using Random = UnityEngine.Random;

namespace TownGenerator.Building
{
    public class CurtainWall
    {



        public Polygon shape;

        // outset shape that is actually used to generate walls
        public Polygon realShape;
        public List<bool> segments;
        public List<Point> gates;
        public List<Point> towers;

        private bool real;
        private List<Patch> patches;

        public CurtainWall(bool real, Model model, List<Patch> patches, List<Point> reserved)
        {
            this.real = true;
            this.patches = patches;

            if (patches.Count == 1)
            {
                shape = patches[0].shape;


            }
            else
            {
                shape = Model.findCircumference(patches);

                if (real)
                {
                    var smoothFactor = Mathf.Min(1, 40 / patches.Count);
                    shape.set(
                         new Polygon(shape.Select(v =>
                            {
                                return reserved.Contains(v) ? v : shape.smoothVertex(v, smoothFactor);
                            }).ToList()
                        )
                    );

                    realShape = shape.buffer(shape.Select(x => Wards.Ward.BUILDING_GAP).ToList());
                }
            }

            buildGates(real, model, reserved);
            segments = shape.Select(x => true).ToList();
        }

        private void buildGates(bool real, Model model, List<Point> reserved)
        {
            gates = new List<Point>();

            // Entrances are vertices of the walls with more than 1 adjacent inner ward
            // so that a street could connect it to the city center
            List<Point> entrances;

            if (patches.Count > 1)
            {
                entrances = shape.Where((v) =>
                {
                    //Debug.Log("Entrance: " + v.vec);

                    var notReserved = !reserved.Contains(v);

                    var sharedPoint = patches.Where(
                        (Patch p) =>
                        {

                            foreach (var shape in p.shape)
                            {
                                //Debug.Log(shape.vec);
                            }

                            return p.shape.FirstOrDefault(point => point.vec == v.vec) != null;
                        }
                    );

                    //Debug.Log("shared : " + sharedPoint.Count());

                    return notReserved && sharedPoint.Count() > 1;

                }).ToList();
            }
            else
            {
                entrances = shape.Where((v) => !reserved.Contains(v)).ToList();
            }


            if (entrances.Count == 0)
                throw new System.Exception("Bad walled area shape!");

            do
            {
                var index = (int)Random.Range(0, entrances.Count);
                var gate = entrances[index];
                gates.Add(gate);

                if (real)
                {
                    var outerWards = model.patchByVertex(gate).Where((Patch w) => !patches.Contains(w)).ToList();
                    if (outerWards.Count() == 1)
                    {
                        // If there is no road leading from the walled patches,
                        // we should make one by splitting an outer ward
                        Patch outer = outerWards[0];
                        if (outer.shape.Count > 3)
                        {
                            var wall = shape.next(gate).vec - (shape.prev(gate)).vec;
                            var outt = new Point(wall.y, -wall.x);

                            var farthest = outer.shape.MaxBy((Point v) =>
                            {

                                if (shape.Contains(v) || reserved.Contains(v))
                                {
                                    return Mathf.NegativeInfinity;

                                }
                                else
                                {
                                    var dir = v.vec - gate.vec;
                                    return Vector2.Dot(dir, outt.vec) / dir.magnitude;
                                }

                            }

                        ).FirstOrDefault();

                            var newPatches = outer.shape.split(gate, farthest).Select(half => new Patch(half)).ToList();

                            var indexx = model.patches.IndexOf(outer);
                            model.patches[indexx++] = newPatches[0];
                            for (var i = 1; i < newPatches.Count; i++)
                                model.patches.Insert(index++, newPatches[i]);
                        }
                    }
                }

                // Removing neighbouring entrances to ensure
                // that no gates are too close

                if (index == 0)
                {
                    if (0 < entrances.Count)
                    {

                        var startIndex = 0;
                        var deleteCount = 2;
                        // assures that delete count doesnt exceed array count
                        entrances.RemoveRange(startIndex, deleteCount - ((deleteCount - 1 + startIndex) - (entrances.Count - 1)));
                    }


                    if (entrances.Count > 0)
                        entrances.RemoveAt(entrances.Count - 1);
                }
                else if (index == entrances.Count - 1)
                {

                    if (index - 1 < entrances.Count)
                    {

                        var startIndex = index - 1;
                        var deleteCount = 2;
                        entrances.RemoveRange(startIndex, deleteCount - ((deleteCount - 1 + startIndex) - (entrances.Count - 1)));
                    }

                    if (entrances.Count > 0)
                        entrances.RemoveAt(0);
                }
                else
                {
                    if (index - 1 < entrances.Count)
                    {

                        var startIndex = index - 1;
                        var deleteCount = 3;
                        entrances.RemoveRange(startIndex, deleteCount - ((deleteCount - 1 + startIndex) - (entrances.Count - 1)));
                    }
                }

            } while (entrances.Count >= 3);

            if (gates.Count == 0)
                throw new System.Exception("Bad walled area shape!");

            // // Smooth further sections of the wall with gates
            // if (real)
            //     for (var i = 0; i < gates.Count; i++)
            //     {
            //         gates[i] = shape.smoothVertex(gates[i]);
            //     }

            // Smooth further sections of the wall with gates
            if (real)
            {
                foreach (var gate in gates)
                {
                    gate.Set(shape.smoothVertex(gate).vec);
                }
            }
        }

        public void buildTowers()
        {
            towers = new List<Point>();
            if (real)
            {
                var len = shape.Count;
                for (var i = 0; i < len; i++)
                {
                    var t = shape[i];
                    if (!gates.Contains(t) && (segments[(i + len - 1) % len] || segments[i]))
                        towers.Add(t);
                }
            }
        }

        public float getRadius()
        {
            var radius = 0.0f;
            foreach (var v in shape)
                radius = Mathf.Max(radius, v.vec.magnitude);
            return radius;
        }

        public bool bordersBy(Patch p, Point v0, Point v1)
        {
            var index = patches.Contains(p) ?
                shape.findEdge(v0, v1) :
                shape.findEdge(v1, v0);
            if (index != -1 && segments[index])
                return true;

            return false;
        }

        public bool borders(Patch p)
        {
            var withinWalls = patches.Contains(p);
            var length = shape.Count;

            for (var i = 0; i < length; i++)
            {
                if (segments[i])
                {
                    var v0 = shape[i];
                    var v1 = shape[(i + 1) % length];
                    var index = withinWalls ?
                        p.shape.findEdge(v0, v1) :
                        p.shape.findEdge(v1, v0);
                    if (index != -1)
                        return true;
                }
            }

            return false;
        }

    }
}