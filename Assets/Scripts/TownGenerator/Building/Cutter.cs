using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TownGenerator.Geom;
using MoreLinq;

namespace TownGenerator.Building
{
    public class Cutter
    {


        public static List<Polygon> bisect(Polygon poly, Point vertex, float ratio = 0.5f, float angle = 0.0f, float gap = 0.0f)
        {

            var next = poly.next(vertex);

            var p1 = GeomUtils.interpolate(vertex, next, ratio);
            var d = next.vec - (vertex.vec);

            var cosB = Mathf.Cos(angle);
            var sinB = Mathf.Sin(angle);
            var vx = d.x * cosB - d.y * sinB;
            var vy = d.y * cosB + d.x * sinB;
            var p2 = new Point(p1.x - vy, p1.y + vx);

            return poly.cut(p1, p2, gap);
        }

        public static List<Polygon> radial(Polygon poly, Point center = null, float gap = 0.0f)
        {

            if (center == null)
                center = poly.centroid;

            var sectors = new List<Polygon>();
            poly.forEdge((v0, v1) =>
            {
                var sector = new Polygon(new List<Point> { center, v0, v1 });
                if (gap > 0)
                    sector = sector.shrink(new List<float> { gap / 2, 0, gap / 2 });

                sectors.Add(sector);
            });
            return sectors;
        }

        public static List<Polygon> semiRadial(Polygon poly, Point center = null, float gap = 0.0f)
        {
            if (center == null)
            {
                var centroid = poly.centroid;
                center = poly.MinBy((Point v) => (v.vec - centroid.vec).magnitude).FirstOrDefault();
            }

            gap /= 2;

            var sectors = new List<Polygon>();
            poly.forEdge((v0, v1) =>
            {
                if (v0 != center && v1 != center)
                {
                    var sector = new Polygon(new List<Point> { center, v0, v1 });
                    if (gap > 0)
                    {
                        var d = new List<float> { poly.findEdge(center, v0) == -1 ? gap : 0, 0, poly.findEdge(v1, center) == -1 ? gap : 0 };
                        sector = sector.shrink(d);
                    }
                    sectors.Add(sector);
                }

            });
            return sectors;
        }

        private class Slice
        {
            public Point p1;
            public Point p2;
            public float len;
        }

        public static List<Polygon> ring(Polygon poly, float thickness)
        {

            var slices = new List<Slice>();
            poly.forEdge((Point v1, Point v2) =>
            {
                var v = v2.vec - (v1.vec);
                var n = new Point(Quaternion.Euler(0, 0, 90) * v.normalized * thickness);
                slices.Add(new Slice
                {
                    p1 = new Point(v1.vec + (n.vec)),
                    p2 = new Point(v2.vec + (n.vec)),
                    len = v.magnitude
                });
            });

            // Short sides should be sliced first
            slices.Sort((s1, s2) => { return (int)(s1.len - s2.len); });

            var peel = new List<Polygon>();

            var p = poly;
            for (var i = 0; i < slices.Count; i++)
            {
                var halves = p.cut(slices[i].p1, slices[i].p2);
                p = halves[0];
                if (halves.Count == 2)
                    peel.Add(halves[1]);
            }

            return peel;
        }


    }
}