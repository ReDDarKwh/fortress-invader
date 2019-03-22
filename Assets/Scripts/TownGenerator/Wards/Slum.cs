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

    public class Slum : CommonWard
    {

        public Slum(Model model, Patch patch)
            : base(model, patch, 10 + 30 * Random.value * Random.value, 0.6f + Random.value * 0.4f, 0.8f, 0.03f)
        {

            // small to large
            // moderately regular
        }

        public static new float rateLocation(Model model, Patch patch)
        {
            // Slums should be as far from the center as possible
            return -patch.shape.distance(model.plaza != null ? model.plaza.shape.center : model.center);
        }

        override public string getLabel()
        {
            return "Slum";
        }

    }
}