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

    public class GateWard : CommonWard
    {

        public GateWard(Model model, Patch patch)
            : base(model, patch, 10 + 50 * Random.value * Random.value, 0.5f + Random.value * 0.1f, 0.7f)
        {

            // small to large
            // moderately regular

        }

        override public string getLabel()
        {
            return "Gate";
        }

    }
}