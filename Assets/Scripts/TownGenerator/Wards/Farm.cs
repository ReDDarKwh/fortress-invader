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

    public class Farm : Ward
    {


        public Farm(Model model, Patch patch) : base(model, patch)
        {
        }


        override public void createGeometry()
        {
            var housing = Polygon.rect(4, 4);
            var pos = GeomUtils.interpolate(patch.shape[Random.Range(0, patch.shape.Count - 1)], patch.shape.centroid, 0.3f + Random.value * 0.4f);
            housing.rotate(Random.value * Mathf.PI);
            housing.offset(pos);

            geometry = Ward.createOrthoBuilding(housing, 8, 0.5f);
        }

        override public String getLabel()
        {
            return "Farm";
        }


    }

}