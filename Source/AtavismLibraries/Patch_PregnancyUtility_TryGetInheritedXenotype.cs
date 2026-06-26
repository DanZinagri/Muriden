using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AtavismLibraries
{
    //By default, the game will think that a parent will birth a "Hybrid"; so we're using a special case in this method
    [HarmonyPatch(typeof(PregnancyUtility), "TryGetInheritedXenotype")]
    public static class Patch_PregnancyUtility_TryGetInheritedXenotype
    {

        [HarmonyPostfix]
        public static void PostFix(Pawn mother, ref XenotypeDef xenotype, ref bool __result)
        {
            if (mother?.genes == null) return;

            if (Patch_PawnGenerator_GeneratePawn.CurrentForcedXenotype != null)
            {
                xenotype = Patch_PawnGenerator_GeneratePawn.CurrentForcedXenotype;
                __result = true;
            }
        }
    }
}