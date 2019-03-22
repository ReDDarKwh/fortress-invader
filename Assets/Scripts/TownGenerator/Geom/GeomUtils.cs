


using UnityEngine;

namespace TownGenerator.Geom
{


    public class GeomUtils
    {
        public static Point intersectLines(float x1, float y1, float dx1, float dy1, float x2, float y2, float dx2, float dy2)
        {
            var d = dx1 * dy2 - dy1 * dx2;
            if (d == 0)
                return null;

            var t2 = (dy1 * (x2 - x1) - dx1 * (y2 - y1)) / d;
            var t1 = dx1 != 0 ?
                (x2 - x1 + dx2 * t2) / dx1 :
                (y2 - y1 + dy2 * t2) / dy1;

            return new Point(t1, t2);
        }

        public static Point interpolate(Point p1, Point p2, float ratio = 0.5f)
        {
            var d = p2.vec - p1.vec;
            return new Point(p1.x + d.x * ratio, p1.y + d.y * ratio);
        }

        public static float scalar(float x1, float y1, float x2, float y2)
        {
            return x1 * x2 + y1 * y2;
        }

        public static float cross(float x1, float y1, float x2, float y2)
        {

            return x1 * y2 - y1 * x2;
        }

        public static float distance2line(float x1, float y1, float dx1, float dy1, float x0, float y0)
        {


            return (dx1 * y0 - dy1 * x0 + (y1 + dy1) * x1 - (x1 + dx1) * y1) / Mathf.Sqrt(dx1 * dx1 + dy1 * dy1);
        }

    }


}