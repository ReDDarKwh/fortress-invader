


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace TownGenerator.Geom
{
    class Segment
    {
        public Point start;
        public Point end;

        public Segment(Point start, Point end)
        {
            this.start = start;
            this.end = end;
        }

        // public Point dx;
        // public float get_dx()
        // {
        //     return (end.x - start.x);
        // }

        // public float dy;
        // public float get_dy()
        // {
        //     return (end.y - start.y);
        // }
        // public Point vector;
        // public Point get_vector()
        // {
        //     return end - (start);
        // }

        // public float length;
        // public float get_length()
        // {
        //     return (start - end).magnitude;
        // }

    }
}