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


    public class PatriciateWard : CommonWard
    {


        public PatriciateWard(Model model, Patch patch) :
         base(model, patch, 80 + 30 * Random.value * Random.value, 0.5f + Random.value * 0.3f, 0.8f, 0.2f)
        {
        }


        public static new float rateLocation(Model model, Patch patch)
        {
            // Patriciate ward prefers to border a park and not to border slums
            var rate = 0;
            foreach (var p in model.patches)
            {
                if (p.ward != null && p.shape.borders(patch.shape))
                {
                    if (p.ward.GetType() == typeof(Park))
                    {
                        rate--;
                    }
                    else if (p.ward.GetType() == typeof(Slum))
                        rate++;
                }
            }
            return rate;
        }

        override public String getLabel()
        {
            return "Patriciate";
        }

    }
}