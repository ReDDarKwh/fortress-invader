

using UnityEngine;

namespace TownGenerator.Geom
{
    public class Point
    {
        public Vector2 vec;


        public float x
        {
            get
            {
                return vec.x;
            }
            set
            {
                vec.x = value;
            }
        }

        public float y
        {
            get
            {
                return vec.y;
            }
            set
            {
                vec.y = value;
            }
        }

        public Point(float x, float y)
        {
            this.vec = new Vector2(x, y);
        }

        public Point()
        {
            this.vec = Vector2.zero;
        }

        public Point(Vector2 vec)
        {
            this.vec = vec;
        }
    }
}
