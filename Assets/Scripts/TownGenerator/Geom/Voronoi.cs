




using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace TownGenerator.Geom
{

    class Voronoi
    {
        public List<Triangle> triangles;

        public Dictionary<Vector2, Region> regions;
        private bool _regionsDirty;
        private Dictionary<Vector2, Region> _regions;

        public List<Vector2> points;
        public List<Vector2> frame;

        public Voronoi(float minx, float miny, float maxx, float maxy)
        {

            triangles = new List<Triangle>();

            var c1 = new Vector2(minx, miny);
            var c2 = new Vector2(minx, maxy);
            var c3 = new Vector2(maxx, miny);
            var c4 = new Vector2(maxx, maxy);
            frame = new List<Vector2> { c1, c2, c3, c4 };
            points = new List<Vector2> { c1, c2, c3, c4 };
            triangles.Add(new Triangle(c1, c2, c3));
            triangles.Add(new Triangle(c2, c3, c4));

            // Maybe we shouldn't do it beause these temporary
            // regions will be discarded anyway
            _regions = null; //points.ToDictionary(x => buildRegion(x),);
            _regionsDirty = false;
        }

        /**
	* Adds a point to the list and updates the list of triangles
	* @param p a point to add
	**/
        public void addPoint(Vector2 p)
        {
            var toSplit = new List<Triangle>();
            foreach (var tr in triangles)
            {
                if ((p - tr.c).magnitude < tr.r)
                {
                    toSplit.Add(tr);
                }
            }

            if (toSplit.Count > 0)
            {

                points.Add(p);

                var a = new List<Vector2>();
                var b = new List<Vector2>();
                foreach (var t1 in toSplit)
                {
                    var e1 = true;
                    var e2 = true;
                    var e3 = true;
                    foreach (var t2 in toSplit) if (t2 != t1)
                        {
                            // If triangles have a common edge, it goes in opposite directions
                            if (e1 && t2.hasEdge(t1.p2, t1.p1)) e1 = false;
                            if (e2 && t2.hasEdge(t1.p3, t1.p2)) e2 = false;
                            if (e3 && t2.hasEdge(t1.p1, t1.p3)) e3 = false;
                            if (!(e1 || e2 || e3)) break;
                        }
                    if (e1) { a.Add(t1.p1); b.Add(t1.p2); }
                    if (e2) { a.Add(t1.p2); b.Add(t1.p3); }
                    if (e3) { a.Add(t1.p3); b.Add(t1.p1); }
                }

                var index = 0;
                do
                {
                    triangles.Add(new Triangle(p, a[index], b[index]));
                    index = a.IndexOf(b[index]);
                } while (index != 0);

                foreach (var tr in toSplit)
                    triangles.Remove(tr);

                _regionsDirty = true;
            }
        }

        private Region buildRegion(Vector2 p)
        {
            var r = new Region(p);
            foreach (var tr in triangles)
                if (tr.p1 == p || tr.p2 == p || tr.p3 == p)
                    r.vertices.Add(tr);

            return r.sortVertices();
        }

        public Dictionary<Vector2, Region> get_regions()
        {
            if (_regionsDirty)
            {
                _regions = new Dictionary<Vector2, Region>();
                _regionsDirty = false;
                foreach (var p in points)
                    _regions[p] = buildRegion(p);
            }
            return _regions;
        }

        /**
        * Checks if neither of a triangle's vertices is a frame point
        **/
        private bool isReal(Triangle tr)
        {

            return !(frame.Contains(tr.p1) || frame.Contains(tr.p2) || frame.Contains(tr.p3));
        }

        /**
        * Returns triangles which do not contain "frame" points as their vertices
        * @return List of triangles
        **/
        public List<Triangle> triangulation()
        {

            return triangles.Where(tr => !(frame.Contains(tr.p1) || frame.Contains(tr.p2) || frame.Contains(tr.p3))).ToList();
        }

        public List<Region> partioning()
        {
            // Iterating over points, not regions, to use points ordering
            var result = new List<Region>();
            foreach (var p in points)
            {
                var r = regions[p];
                var isReal = true;
                foreach (var v in r.vertices)
                    if (!this.isReal(v))
                    {
                        isReal = false;
                        break;
                    }

                if (isReal)
                    result.Add(r);
            }
            return result;
        }

        public List<Region> getNeighbours(Region r1)
        {
            // return [for (r2 in regions.iterator()) if (r1.borders(r2)) r2];
            return regions.Values.Where(r2 => r1.borders(r2)).ToList();
        }

        public static Voronoi relax(Voronoi voronoi, List<Vector2> toRelax = null)
        {
            var regions = voronoi.partioning();

            var points = new List<Vector2>(voronoi.points);
            foreach (var p in voronoi.frame) points.Remove(p);

            if (toRelax == null) toRelax = voronoi.points;
            foreach (var r in regions)
                if (toRelax.Contains(r.seed))
                {
                    points.Remove(r.seed);
                    points.Add(r.center());
                }

            return build(points);
        }

        public static Voronoi build(List<Vector2> vertices)
        {
            var minx = 1e+10;
            var miny = 1e+10;
            var maxx = -1e+9;
            var maxy = -1e+9;
            foreach (var v in vertices)
            {
                if (v.x < minx) minx = v.x;
                if (v.y < miny) miny = v.y;
                if (v.x > maxx) maxx = v.x;
                if (v.y > maxy) maxy = v.y;
            }
            var dx = (maxx - minx) * 0.5;
            var dy = (maxy - miny) * 0.5;

            var voronoi = new Voronoi(
                (float)(minx - dx / 2),
                (float)(miny - dy / 2),
                (float)(maxx + dx / 2),
                (float)(maxy + dy / 2));
            foreach (var v in vertices)
                voronoi.addPoint(v);

            return voronoi;
        }
    }

    public class Triangle
    {
        public Vector2 p1;
        public Vector2 p2;
        public Vector2 p3;

        public Vector2 c;
        public float r;

        public Triangle(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            var s = (p2.x - p1.x) * (p2.y + p1.y) + (p3.x - p2.x) * (p3.y + p2.y) + (p1.x - p3.x) * (p1.y + p3.y);
            this.p1 = p1;
            // CCW
            this.p2 = s > 0 ? p2 : p3;
            this.p3 = s > 0 ? p3 : p2;

            var x1 = (p1.x + p2.x) / 2;
            var y1 = (p1.y + p2.y) / 2;
            var x2 = (p2.x + p3.x) / 2;
            var y2 = (p2.y + p3.y) / 2;

            var dx1 = p1.y - p2.y;
            var dy1 = p2.x - p1.x;
            var dx2 = p2.y - p3.y;
            var dy2 = p3.x - p2.x;

            var tg1 = dy1 / dx1;
            var t2 = ((y1 - y2) - (x1 - x2) * tg1) /
                        (dy2 - dx2 * tg1);

            c = new Vector2(x2 + dx2 * t2, y2 + dy2 * t2);
            r = (c - p1).magnitude;
        }

        public bool hasEdge(Vector2 a, Vector2 b)
        {
            return
                (p1 == a && p2 == b) ||
                (p2 == a && p3 == b) ||
                (p3 == a && p1 == b);
        }
    }

    class Region
    {
        public Vector2 seed;
        public List<Triangle> vertices;

        public Region(Vector2 seed)
        {
            this.seed = seed;
            vertices = new List<Triangle>();
        }

        public Region sortVertices()
        {
            vertices.Sort(compareAngles);
            return this;
        }

        public Vector2 center()
        {
            var c = Vector2.zero;
            foreach (var v in vertices) c += v.c;

            return c * (1 / vertices.Count);
        }

        public bool borders(Region r)
        {
            var len1 = vertices.Count;
            var len2 = r.vertices.Count;
            for (var i = 0; i < len1; i++)
            {
                var j = r.vertices.IndexOf(vertices[i]);
                if (j != -1)
                    return vertices[(i + 1) % len1] == r.vertices[(j + len2 - 1) % len2];
            }
            return false;
        }

        private int compareAngles(Triangle v1, Triangle v2)
        {
            //	return MathUtils.sign( v1.c.subtract( seed ).atan() - v2.c.subtract( seed ).atan() );
            var x1 = v1.c.x - seed.x;
            var y1 = v1.c.y - seed.y;
            var x2 = v2.c.x - seed.x;
            var y2 = v2.c.y - seed.y;

            if (x1 >= 0 && x2 < 0) return 1;
            if (x2 >= 0 && x1 < 0) return -1;
            if (x1 == 0 && x2 == 0)
                return y2 > y1 ? 1 : -1;

            return (int)(Mathf.Sign(x2 * y1 - x1 * y2));
        }
    }
}