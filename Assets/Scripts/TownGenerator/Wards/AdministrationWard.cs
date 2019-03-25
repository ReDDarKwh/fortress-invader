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


    public class AdministrationWard : CommonWard
    {

        public AdministrationWard(Model model, Patch patch) :
        base(model, patch, 80 + 30 * Random.value * Random.value, 0.1f + Random.value * 0.3f, 0.3f)
        {
        }


        public static new float rateLocation(Model model, Patch patch)
        {
            // Ideally administration ward should overlook the plaza,
            // otherwise it should be as close to the plaza as possible
            return model.plaza != null ?
                (patch.shape.borders(model.plaza.shape) ? 0 : patch.shape.distance(model.plaza.shape.center)) :
                patch.shape.distance(model.center);
        }

        override public String getLabel()
        {
            return "Administration";
        }

    }
}