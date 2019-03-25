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


    public class MerchantWard : CommonWard
    {

        public MerchantWard(Model model, Patch patch) :
         base(model, patch, 50 + 60 * Random.value * Random.value, 0.5f + Random.value * 0.3f, 0.7f, 0.15f)
        {
        }


        public static new float rateLocation(Model model, Patch patch)
        {
            // Merchant ward should be as close to the center as possible
            return patch.shape.distance(model.plaza != null ? model.plaza.shape.center : model.center);
        }

        override public String getLabel()
        {
            return "Merchant";
        }

    }
}