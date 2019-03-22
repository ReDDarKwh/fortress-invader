

using System;
using System.Collections.Generic;
using TownGenerator.Building;

using TownGenerator.Geom;
using System.Linq;
using UnityEngine;

using MoreLinq;

namespace TownGenerator.Wards
{


    public class Castle : Ward
    {

        public CurtainWall wall;

        public Castle(Model model, Patch patch) : base(model, patch)
        {


            wall = new CurtainWall(true, model, new List<Patch> { patch }, patch.shape.Where(
                (Point v) =>
                {
                    return model.patchByVertex(v).Any(
                        (Patch p) => { return !p.withinCity; }
                    );
                }
            ).ToList());


        }

        override public void createGeometry()
        {
            var block = patch.shape.shrinkEq(Ward.MAIN_STREET * 2);
            geometry = Ward.createOrthoBuilding(block, Mathf.Sqrt(block.square) * 4, 0.6f);
        }

        override public string getLabel()
        {
            return "Castle";
        }
    }

}