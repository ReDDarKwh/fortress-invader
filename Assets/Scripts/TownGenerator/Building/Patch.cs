using System;
using System.Collections;
using System.Collections.Generic;
using TownGenerator.Geom;
using TownGenerator.Wards;
using UnityEngine;
using System.Linq;

namespace TownGenerator.Building
{
    public class Patch
    {
        public Polygon shape;
        public Ward ward;

        public bool withinWalls;
        public bool withinCity;

        public Patch(List<Point> vertices)
        {
            this.shape = new Polygon(vertices);
            withinCity = false;
            withinWalls = false;
        }
        public static Patch fromRegion(Region r)
        {
            return new Patch(r.vertices.Select(x =>
            {

                if (float.IsNaN(x.c.x))
                {

                    Debug.Log("wyh?");
                }

                return x.c;

            }).ToList());
        }
    }
}
