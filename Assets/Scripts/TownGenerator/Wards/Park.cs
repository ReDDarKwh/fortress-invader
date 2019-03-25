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


    public class Park : Ward
    {

        public Park(Model model, Patch patch) : base(model, patch)
        {
        }


        override public void createGeometry()
        {
            var block = getCityBlock();
            geometry = block.compactness >= 0.7 ?
                Cutter.radial(block, null, Ward.ALLEY) :
                Cutter.semiRadial(block, null, Ward.ALLEY);
        }

        override public String getLabel()
        {
            return "Park";
        }

    }


}