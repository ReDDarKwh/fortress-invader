using System.Collections.Generic;
using TownGenerator.Wards;
using UnityEngine;

namespace TownGenerator.Building
{
    public class Model
    {

        public static Model instance;

        // Small Town	6
        // Large Town	10
        // Small City	15
        // Large City	24
        // Metropolis	40
        private int nPatches;

        private bool plazaNeeded;
        private bool citadelNeeded;
        private bool wallsNeeded;

        public static List<Ward> WARDS = new List<Ward> {
            CraftsmenWard, CraftsmenWard, MerchantWard, CraftsmenWard, CraftsmenWard, Cathedral,
            CraftsmenWard, CraftsmenWard, CraftsmenWard, CraftsmenWard, CraftsmenWard,
            CraftsmenWard, CraftsmenWard, CraftsmenWard
        };

    }
}