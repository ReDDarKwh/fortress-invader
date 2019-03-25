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


    public class MilitaryWard : Ward
    {


        public MilitaryWard(Model model, Patch patch) : base(model, patch)
        {
        }

        override public void createGeometry()
        {
            var block = getCityBlock();
            geometry = Ward.createAlleys(block,
                Mathf.Sqrt(block.square) * (1 + Random.value),
            0.1f + Random.value * 0.3f, 0.3f,            // regular
            0.25f);                                     // squares
        }

        public static new float rateLocation(Model model, Patch patch)
        {
            // Military ward should border the citadel or the city walls
            if (model.citadel != null && model.citadel.shape.borders(patch.shape))
            { return 0; }

            else if (model.wall != null && model.wall.borders(patch))
            { return 1; }
            else
                return (model.citadel == null && model.wall == null ? 0 : Mathf.Infinity);
        }

        override public String getLabel()
        {
            return "Military";
        }
    }
}