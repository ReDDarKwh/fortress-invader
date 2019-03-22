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


    public class CraftsmenWard : CommonWard
    {

        public CraftsmenWard(Model model, Patch patch)
        : base(model, patch, 10 + 80 * Random.value * Random.value, 0.5f + Random.value * 0.2f, 0.6f)
        {

            // small to large
            // moderately regular

        }


        override public string getLabel()
        {
            return "Craftsmen";
        }

    }

}