



using System.Collections.Generic;
using UnityEngine;

namespace TownGenerator.Geom
{

    public class Polygon : List<Vector2>
    {

        public Polygon(List<Vector2> vertices = null)
        {
            this.AddRange((vertices != null ? new List<Vector2>(vertices) : new List<Vector2>()));
        }

    }

}