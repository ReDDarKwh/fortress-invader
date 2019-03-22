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


    public class Cathedral : Ward
    {

        public Cathedral(Model model, Patch patch) : base(model, patch)
        {

        }

        public override void createGeometry()
        {


            geometry = Random.value < (0.4) ?
                Cutter.ring(getCityBlock(), 2 + Random.value * 4) :
                Ward.createOrthoBuilding(getCityBlock(), 50, 0.8f);

        }

        public static new float  rateLocation(Model model, Patch patch)
        {

            // Ideally the main temple should overlook the plaza,
            // otherwise it should be as close to the plaza as possible
            if (model.plaza != null && patch.shape.borders(model.plaza.shape))
            {

                return -1 / patch.shape.square;
            }

            else
            {
                return patch.shape.distance(model.plaza != null ? model.plaza.shape.center : model.center) * patch.shape.square;
            }
        }

        public override string getLabel()
        {
            return "Temple";
        }


    }

}