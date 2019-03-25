



using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace TownGenerator.Geom
{


    public class Polygon : List<Point>
    {

        private static float DELTA = 0.000001f;

        public Polygon(List<Point> vertices = null)
        {
            this.AddRange((vertices != null ? new List<Point>(vertices) : new List<Point>()));
        }


        public void set(Polygon p)
        {
            for (var i = 0; i < p.Count; i++)
            {
                this[i].Set(p[i].vec);
            }
        }

        public float square
        {
            get
            {
                return _square();
            }
        }


        public float _square()
        {
            var v1 = this[this.Count - 1].vec;
            var v2 = this[0].vec;
            var s = v1.x * v2.y - v2.x * v1.y;
            for (var i = 1; i < this.Count; i++)
            {
                v1 = v2;
                v2 = this[i].vec;
                s += (v1.x * v2.y - v2.x * v1.y);
            }
            return s * 0.5f;
        }

        public float perimeter
        {
            get
            {
                return _perimeter();
            }
        }
        public float _perimeter()
        {
            var len = 0.0;
            forEdge((Point v0, Point v1) =>
        {
            len += (v0.vec - v1.vec).magnitude;
        });
            return (float)len;
        }

        // for circle	= 1.00
        // for square	= 0.79
        // for triangle	= 0.60
        public float compactness
        {
            get
            {
                return _compactness();
            }
        }
        public float _compactness()
        {
            var p = perimeter;
            return 4 * Mathf.PI * square / (p * p);
        }

        // Faster approximation of centroid
        public Point center { get { return _center(); } }
        public Point _center()
        {
            var c = new Point();
            foreach (var v in this)
            {
                c.vec += v.vec;
            }

            c.vec /= this.Count;
            return c;
        }

        public Point centroid
        {

            get
            {
                return _centroid();
            }
        }
        public Point _centroid()
        {
            var x = 0.0;
            var y = 0.0;
            var a = 0.0;
            forEdge((Point v0, Point v1) =>
            {
                var f = GeomUtils.cross(v0.x, v0.y, v1.x, v1.y);
                a += f;
                x += (v0.x + v1.x) * f;
                y += (v0.y + v1.y) * f;
            });
            var s6 = 1 / (3 * a);
            return new Point((float)(s6 * x), (float)(s6 * y));
        }

        public bool contains(Point v)
        {
            return this.IndexOf(v) != -1;
        }

        public void forEdge(Action<Point, Point> f)
        {

            var len = this.Count;
            for (var i = 0; i < len; i++)
                f(this[i], this[(i + 1) % len]);
        }

        // Similar to forEdge, but doesn't iterate over the v(n)-v(0)
        public void forSegment(Action<Point, Point> f)
        {
            for (var i = 0; i < this.Count - 1; i++)
            {
                f(this[i], this[i + 1]);
            }
        }

        public void offset(Point p)
        {
            var dx = p.x;
            var dy = p.y;
            var offset = new Point(dx, dy);
            for (var i = 0; i < this.Count; i++)
            {
                this[i].vec += offset.vec;
            }
        }

        public void rotate(float a)
        {
            var cosA = Mathf.Cos(a);
            var sinA = Mathf.Sin(a);
            for (var i = 0; i < this.Count; i++)
            {
                var vx = this[i].x * cosA - this[i].y * sinA;
                var vy = this[i].y * cosA + this[i].x * sinA;
                this[i] = new Point(vx, vy);
            }
        }

        public bool isConvexVertexi(int i)
        {
            var len = this.Count;
            var v0 = this[(i + len - 1) % len].vec;
            var v1 = this[i].vec;
            var v2 = this[(i + 1) % len].vec;
            return GeomUtils.cross(v1.x - v0.x, v1.y - v0.y, v2.x - v1.x, v2.y - v1.y) > 0;
        }

        public bool isConvexVertex(Point v1)
        {
            var v0 = prev(v1).vec;
            var v2 = next(v1).vec;
            return GeomUtils.cross(v1.x - v0.x, v1.y - v0.y, v2.x - v1.x, v2.y - v1.y) > 0;
        }

        public bool isConvex()
        {
            foreach (var v in this)
                if (!isConvexVertex(v)) return false;
            return true;
        }

        public Point smoothVertexi(int i, float f = 1.0f)
        {
            var v = this[i].vec;
            var len = this.Count;
            var prev = this[(i + len - 1) % len].vec;
            var next = this[(i + 1) % len].vec;
            var result = new Point(
                (prev.x + v.x * f + next.x) / (2 + f),
                (prev.y + v.y * f + next.y) / (2 + f)
            );
            return result;
        }

        public Point smoothVertex(Point v, float f = 1.0f)
        {
            var prevv = prev(v).vec;
            var nextt = next(v).vec;
            return new Point(new Point(
                prevv.x + v.x * f + nextt.x,
                prevv.y + v.y * f + nextt.y

            ).vec / ((2 + f)));
        }

        // This  returns minimal distance from any of the vertices
        // to a point, not real distance from the polygon
        public float distance(Point p)
        {
            var d = Mathf.Infinity;
            for (var i = 0; i < this.Count; i++)
            {
                var v1 = this[i];
                var d1 = (v1.vec - p.vec).magnitude;

                if (d1 < d)
                {
                    d = d1;
                }
            }
            return d;
        }

        public Polygon smoothVertexEq(float f = 1.0f)
        {
            var len = this.Count;
            var v1 = this[len - 1];
            var v2 = this[0];

            var result = new List<Point>();

            for (var i = 1; i < len; i++)
            {
                var v0 = v1; v1 = v2; v2 = this[(i + 1) % len];
                result.Add(new Point(
                    (v0.x + v1.x * f + v2.x) / (2 + f),
                    (v0.y + v1.y * f + v2.y) / (2 + f)
                ));
            }

            return new Polygon(result);
        }

        public Polygon filterShort(float threshold)
        {
            var i = 1;
            var v0 = this[0];
            var v1 = this[1];
            var result = new List<Point> { v0 };
            do
            {
                do
                {
                    v1 = this[i++];
                } while ((v0.vec - v1.vec).magnitude < threshold && i < this.Count);
                result.Add(v0 = v1);
            } while (i < this.Count);

            return new Polygon(result);
        }

        // This  insets one edge defined by its first vertex.
        // It's not very relyable, but it usually works (better for convex
        // vertices than for concave ones). It doesn't change the number
        // of vertices.
        public void inset(Point p1, float d)
        {
            var i1 = this.IndexOf(p1);
            var i0 = (i1 > 0 ? i1 - 1 : this.Count - 1); var p0 = this[i0];
            var i2 = (i1 < this.Count - 1 ? i1 + 1 : 0); var p2 = this[i2];
            var i3 = (i2 < this.Count - 1 ? i2 + 1 : 0); var p3 = this[i3];

            var v0 = p1.vec - p0.vec;
            var v1 = p2.vec - p1.vec;
            var v2 = p3.vec - p2.vec;

            var cos = Vector2.Dot(v0, v1) / v0.magnitude / v1.magnitude;
            var z = v0.x * v1.y - v0.y * v1.x;
            var t = d / Mathf.Sqrt(1 - cos * cos); // sin( acos( cos ) )
            if (z > 0)
            {
                t = Mathf.Min(t, v0.magnitude * 0.99f);
            }
            else
            {
                t = Mathf.Min(t, v1.magnitude * 0.5f);
            }
            t *= Mathf.Sign(z);

            this[i1] = new Point(p1.vec - (v0.normalized * (t)));

            cos = Vector2.Dot(v1, v2) / v1.magnitude / v2.magnitude;
            z = v1.x * v2.y - v1.y * v2.x;
            t = d / Mathf.Sqrt(1 - cos * cos);
            if (z > 0)
            {
                t = Mathf.Min(t, v2.magnitude * 0.99f);
            }
            else
            {
                t = Mathf.Min(t, v1.magnitude * 0.5f);
            }
            this[i2] = new Point(p2.vec + (v2.normalized * (t)));
        }


        public Polygon insetAll(List<float> d)
        {
            var p = new Polygon(this);
            for (var i = 0; i < p.Count; i++)
                if (d[i] != 0) p.inset(p[i], d[i]);
            return p;
        }

        // This  insets all edges by the same distance
        public void insetEq(float d)
        {
            for (var i = 0; i < this.Count; i++)
                inset(this[i], d);
        }

        // This  insets all edges by distances defined in an array.
        // It's kind of reliable for both convex and concave vertices, but only
        // if all distances are equal. Otherwise weird "steps" are created.
        // It does change the number of vertices.
        public Polygon buffer(List<float> d)
        {
            // Creating a polygon (probably invalid) with offset edges
            var q = new Polygon();
            var k = 0;
            forEdge((Point v0, Point v1) =>
        {
            var dd = d[k++];
            if (dd == 0)
            {
                q.Add(v0);
                q.Add(v1);
            }
            else
            {
                // here we may want to do something fancier for nicer joints
                var v = v1.vec - v0.vec;
                var n = (Vector2)(Quaternion.Euler(0, 0, 90) * v.normalized * (dd));
                q.Add(new Point(v0.vec + n));
                q.Add(new Point(v1.vec + n));
            }
        });

            // Creating a valid polygon by dealing with self-intersection:
            // we need to find intersections of every edge with every other edge
            // and add intersection point (twice - for one edge and for the other)
            bool wasCut;
            var lastEdge = 0;
            do
            {
                wasCut = false;

                var n = q.Count;
                for (var ii = lastEdge; ii < n - 2; ii++)
                {
                    lastEdge = ii;

                    var p11 = q[ii];
                    var p12 = q[ii + 1];
                    var x1 = p11.x;
                    var y1 = p11.y;
                    var dx1 = p12.x - x1;
                    var dy1 = p12.y - y1;

                    for (var j = ii + 2; j < (ii > 0 ? n : n - 1); j++)
                    {
                        var p21 = q[j];
                        var p22 = j < n - 1 ? q[j + 1] : q[0];
                        var x2 = p21.x;
                        var y2 = p21.y;
                        var dx2 = p22.x - x2;
                        var dy2 = p22.y - y2;

                        var int1 = GeomUtils.intersectLines(x1, y1, dx1, dy1, x2, y2, dx2, dy2);
                        if (int1 != null && int1?.x > DELTA && int1?.x < 1 - DELTA && int1?.y > DELTA && int1?.y < 1 - DELTA)
                        {
                            var pn = new Point(x1 + dx1 * int1.x, y1 + dy1 * int1.x);

                            q.Insert(j + 1, pn);
                            q.Insert(ii + 1, pn);

                            wasCut = true;
                            break;
                        }
                    }
                    if (wasCut) break;
                }

            } while (wasCut);


            // Checking every part of the polygon to pick the biggest
            var regular = new List<int>();

            for (var i = 0; i < q.Count; i++)
            {
                regular.Add(i);
            }

            var bestPart = new List<Point>();
            var bestPartSq = Mathf.NegativeInfinity;

            while (regular.Count > 0)
            {
                var indices = new List<int>();
                var start = regular[0];
                var i = start;
                do
                {
                    indices.Add(i);
                    regular.Remove(i);

                    var next = (i + 1) % q.Count;
                    var v = q[next];
                    var next1 = q.IndexOf(v);
                    if (next1 == next)
                        next1 = q.LastIndexOf(v);
                    i = next1 == -1 ? next : next1;
                } while (i != start);

                var p = new Polygon(indices.Select(n => q[n]).ToList());

                var s = p.square;
                if (s > bestPartSq)
                {
                    bestPart = p;
                    bestPartSq = s;
                }
            }

            return new Polygon(bestPart);
        }

        // Another version of "buffer"  for insetting all edges
        // by the same distance (it's the best use of that  anyway)
        public Polygon bufferEq(float d)
        {

            return buffer(this.Select(x => d).ToList());
        }

        // This  insets all edges by distances defined in an array.
        // It can't outset a polygon. Works very well for convex polygons,
        // not so much concaqve ones. It produces a convex polygon.
        // It does change the number vertices
        public Polygon shrink(List<float> d)
        {
            var q = new Polygon(this);
            var i = 0;
            forEdge((Point v1, Point v2) =>
        {
            var dd = d[i++];
            if (dd > 0)
            {
                var v = v2.vec - v1.vec;
                var n = (Vector2)(Quaternion.Euler(0, 0, 90) * v.normalized * (dd));
                q = q.cut(new Point(v1.vec + n), new Point(v2.vec + n), 0)[0];
            }
        });
            return q;
        }

        public Polygon shrinkEq(float d)
        {
            return shrink(this.Select(x => d).ToList());
        }

        // A version of "shrink"  for insetting just one edge.
        // It effectively cuts a peel along the edge.
        public Polygon peel(Point v1, float d)
        {
            var i1 = this.IndexOf(v1);
            var i2 = i1 == this.Count - 1 ? 0 : i1 + 1;
            var v2 = this[i2];

            var v = v2.vec - (v1.vec);
            var n = (Vector2)(Quaternion.Euler(0, 0, 90) * v.normalized * (d));

            return cut(new Point(v1.vec + (n)), new Point(v2.vec + (n)), 0)[0];
        }

        // Simplifies the polygons leaving only n vertices
        public void simplyfy(int n)
        {
            var len = this.Count;
            while (len > n)
            {
                var result = 0;
                var min = Mathf.Infinity;

                var b = this[len - 1];
                var c = this[0];
                for (var i = 0; i < len; i++)
                {
                    var a = b;
                    b = c;
                    c = this[(i + 1) % len];
                    var measure = Mathf.Abs(a.x * (b.y - c.y) + b.x * (c.y - a.y) + c.x * (a.y - b.y));
                    if (measure < min)
                    {
                        result = i;
                        min = measure;
                    }
                }

                this.RemoveAt(result);
                len--;
            }
        }

        public int findEdge(Point a, Point b)
        {
            var index = this.IndexOf(a);
            return ((index != -1 && this[(index + 1) % this.Count] == b) ? index : -1);
        }

        public Point next(Point a)
        {
            return this[(this.IndexOf(a) + 1) % this.Count];
        }

        public Point prev(Point a)
        {
            return this[(this.IndexOf(a) + this.Count - 1) % this.Count];
        }

        public Point vector(Point v)
        {
            return new Point(next(v).vec - (v).vec);
        }

        public Point vectori(int i)
        {
            return new Point(this[i == this.Count - 1 ? 0 : i + 1].vec - (this[i]).vec);
        }


        public bool borders(Polygon another)
        {
            var len1 = this.Count;
            var len2 = another.Count;
            for (var i = 0; i < len1; i++)
            {
                var j = another.IndexOf(this[i]);
                if (j != -1)
                {
                    var next = this[(i + 1) % len1];
                    // If this cause is not true, then should return false,
                    // but it doesn't work for some reason
                    if (next == another[(j + 1) % len2] || next == another[(j + len2 - 1) % len2])
                        return true;
                }
            }
            return false;
        }


        public Rect getBounds()
        {
            var rect = new Rect(this[0].x, this[0].y, 0, 0);
            foreach (var v in this)
            {
                rect.xMin = Mathf.Min(rect.xMin, v.x);
                rect.xMax = Mathf.Max(rect.xMax, v.x);
                rect.yMin = Mathf.Min(rect.yMin, v.y);
                rect.yMax = Mathf.Max(rect.yMax, v.y);
            }
            return rect;
        }

        public List<Polygon> split(Point p1, Point p2)
        {

            return spliti(this.IndexOf(p1), this.IndexOf(p2));
        }

        public List<Polygon> spliti(int i1, int i2)
        {
            if (i1 > i2)
            {
                var t = i1;
                i1 = i2;
                i2 = t;
            }

            return new List<Polygon>{

                new Polygon(this.Skip(i1).Take((i2+1) - i1).ToList()),
                new Polygon(this.Skip(i2).Concat(this.Take(i1 + 1).ToList()).ToList())
            };
        }

        public List<Polygon> cut(Point p1, Point p2, float gap = 0)
        {
            var x1 = p1.x;
            var y1 = p1.y;
            var dx1 = p2.x - x1;
            var dy1 = p2.y - y1;

            var len = this.Count;
            var edge1 = 0;
            var ratio1 = 0.0f;
            var edge2 = 0;
            var ratio2 = 0.0f;
            var count = 0;

            for (var i = 0; i < len; i++)
            {
                var v0 = this[i];
                var v1 = this[(i + 1) % len];

                var x2 = v0.x;
                var y2 = v0.y;
                var dx2 = v1.x - x2;
                var dy2 = v1.y - y2;

                var t = GeomUtils.intersectLines(x1, y1, dx1, dy1, x2, y2, dx2, dy2);
                if (t != null && t?.y >= 0 && t?.y <= 1)
                {
                    switch (count)
                    {
                        case 0:
                            edge1 = i;
                            ratio1 = t.x;
                            break;
                        case 1:
                            edge2 = i;
                            ratio2 = t.x;
                            break;
                    }
                    count++;
                }
            }

            if (count == 2)
            {
                var point1 = new Point(p1.vec + ((p2.vec - p1.vec) * (ratio1)));
                var point2 = new Point(p1.vec + ((p2.vec - p1.vec) * (ratio2)));

                var half1 = new Polygon(this.Skip(edge1 + 1).Take((edge2 + 1) - (edge1 + 1)).ToList());
                half1.Insert(0, point1);
                half1.Add(point2);

                var half2 = new Polygon(this.Skip(edge2 + 1).Concat(this.Take(edge1 + 1)).ToList());
                half2.Insert(0, point2);
                half2.Add(point1);

                if (gap > 0)
                {
                    half1 = half1.peel(point2, gap / 2);
                    half2 = half2.peel(point1, gap / 2);
                }

                var v = vectori(edge1);
                return GeomUtils.cross(dx1, dy1, v.x, v.y) > 0 ? new List<Polygon> { half1, half2 } : new List<Polygon> { half2, half1 };
            }
            else
                return new List<Polygon> { new Polygon(this) };
        }

        public List<float> interpolate(Point p)
        {
            var sum = 0.0;
            var dd = this.Select(v =>
            {
                var d = 1 / (v.vec - p.vec).magnitude;
                sum += d;
                return d;
            });

            return dd.Select(d => (float)(d / sum)).ToList();
        }

        public static Polygon rect(float w = 1.0f, float h = 1.0f)
        {
            return new Polygon(new List<Point>{

                new Point(-w / 2, -h / 2),
                new Point(w / 2, -h / 2),
                new Point(w / 2, h / 2),
                new Point(-w / 2, h / 2)
            });
        }

        public static Polygon regular(float n = 8, float r = 1.0f)
        {

            var l = new List<Point>();
            for (var i = 0; i < n; i++)
            {
                var a = i / n * Mathf.PI * 2;
                l.Add(new Point(r * Mathf.Cos(a), r * Mathf.Sin(a)));
            }

            return new Polygon(l);
        }

        public static Polygon circle(float r = 1)
        {
            return regular(16, r);
        }

    }

}