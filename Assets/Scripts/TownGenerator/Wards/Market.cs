




using System;
using System.Collections.Generic;
using TownGenerator.Building;

using TownGenerator.Geom;
using System.Linq;
using UnityEngine;

using MoreLinq;
using Random = UnityEngine.Random;

namespace TownGenerator.Wards
{
    public class Market : Ward
    {


        public Market(Model model, Patch patch) : base(model, patch)
        {

        }

        override public void createGeometry()
        {

            // fountain or statue
            var statue = Random.value < (0.6);
            // we always offset a statue and sometimes a fountain
            var offset = statue || Random.value < (0.3);

            Point v0 = null;
            Point v1 = null;
            if (statue || offset)
            {
                // we need an edge both for rotating a statue and offsetting
                var length = -1.0;
                patch.shape.forEdge((p0, p1) =>
                {
                    var len = (p0.vec - p1.vec).magnitude;
                    if (len > length)
                    {
                        length = len;
                        v0 = p0;
                        v1 = p1;
                    }
                });
            }

            Polygon obj;
            if (statue)
            {
                obj = Polygon.rect(1 + Random.value, 1 + Random.value);
                obj.rotate(Mathf.Atan2(v1.y - v0.y, v1.x - v0.x));
            }
            else
            {
                obj = Polygon.circle(1 + Random.value);
            }

            if (offset)
            {
                var gravity = GeomUtils.interpolate(v0, v1);
                obj.offset(GeomUtils.interpolate(patch.shape.centroid, gravity, 0.2f + Random.value * 0.4f));
            }
            else
            {
                obj.offset(patch.shape.centroid);
            }

            geometry = new List<Polygon> { obj };
        }

        public static new float rateLocation(Model model, Patch patch)
        {
            // One market should not touch another
            foreach (var p in model.inner)
            {

                if (p.ward != null && p.ward.GetType() == typeof(Market) && p.shape.borders(patch.shape))
                {

                    return Mathf.Infinity;

                }

            }

            // Market shouldn't be much larger than the plaza
            return model.plaza != null ? patch.shape.square / model.plaza.shape.square : patch.shape.distance(model.center);
        }

        override public string getLabel()
        {
            return "Market";
        }

    }
}