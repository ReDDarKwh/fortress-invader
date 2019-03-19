



using System.Collections.Generic;
using TownGenerator.Building;
using TownGenerator.Geom;
using UnityEngine;

namespace TownGenerator.Wards
{

    public class Ward
    {
        public static float MAIN_STREET = 2.0f;
        public static float REGULAR_STREET = 1.0f;
        public static float ALLEY = 0.6f;

        public Model model;
        public Patch patch;

        public List<Polygon> geometry;


        public Ward(Model model, Patch patch)
        {
            this.model = model;
            this.patch = patch;
        }

        public void CreateGeometry()
        {
            geometry = new List<Polygon>();
        }



    }

}