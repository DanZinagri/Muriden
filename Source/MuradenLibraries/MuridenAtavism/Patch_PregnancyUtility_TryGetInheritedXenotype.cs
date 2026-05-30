using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace MuridenLibraries
{
    //By default, the game will think that a parent will birth a "Hybrid"; so we're using a special case in this method
    [HarmonyPatch(typeof(PregnancyUtility), "TryGetInheritedXenotype")]
    public static class Patch_PregnancyUtility_TryGetInheritedXenotype
    {

        [HarmonyPostfix]
        public static void PostFix(Pawn mother, ref XenotypeDef xenotype, ref bool __result)
        {
            if (mother == null || mother.genes == null) return;
            if ((AtavismUtils.HasActiveGene(mother, ML_DefOf.Muriden_Resolve) && AtavismUtils.IsMuriden(mother)) || (AtavismUtils.HasActiveGene(mother, ML_DefOf.FeralMuriden_Resolve) && AtavismUtils.IsFeralMuriden(mother)))
            {
                if (Patch_PawnGenerator_GeneratePawn.Atavism)
                {
                    xenotype = ML_DefOf.FeralMuriden;
                }
                else
                {
                    xenotype = ML_DefOf.Muriden;
                }
                __result = true;
            }else if ((AtavismUtils.HasActiveGene(mother, ML_DefOf.Choerites_Calm) && AtavismUtils.IsChoerites(mother)) || (AtavismUtils.HasActiveGene(mother, ML_DefOf.Artigasen_Might) && AtavismUtils.IsArtigasen(mother)))
            {
                if (Patch_PawnGenerator_GeneratePawn.Atavism)
                {
                    xenotype = ML_DefOf.Artigasen;
                }
                else
                {
                    xenotype = ML_DefOf.Choerites;
                }
                __result = true;
            }
            return;

        }
    }
}