using System;
using System.Collections.Generic;
using TownGenerator.Building;

using TownGenerator.Geom;
using System.Linq;
using UnityEngine;

using MoreLinq;

namespace TownGenerator.Wards
{

    public class CommonWard : Ward
    {


        private float minSq;
        private float gridChaos;
        private float sizeChaos;
        private float emptyProb;

        public CommonWard(Model model, Patch patch, float minSq, float gridChaos, float sizeChaos, float emptyProb = 0.04f) : base(model, patch)
        {

            this.minSq = minSq;
            this.gridChaos = gridChaos;
            this.sizeChaos = sizeChaos;
            this.emptyProb = emptyProb;
        }

        override public void createGeometry()
        {
            var block = getCityBlock();
            geometry = Ward.createAlleys(block, minSq, gridChaos, sizeChaos, emptyProb);

            if (!model.isEnclosed(patch))
                filterOutskirts();
        }

    }

}