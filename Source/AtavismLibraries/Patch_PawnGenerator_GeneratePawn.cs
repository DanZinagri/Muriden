using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using static AtavismLibraries.AtavismUtils;

namespace AtavismLibraries
{
    //Patch to apply a chance for the pawn to be born as an alternate xenotype on a chance.
    [HarmonyPatch(typeof(PawnGenerator), "GeneratePawn", new Type[] { typeof(PawnGenerationRequest) })]
    public static class Patch_PawnGenerator_GeneratePawn
    {
        public static bool Atavism = false;
        public static XenotypeDef CurrentForcedXenotype = null;

        [HarmonyPrefix]
        public static void Prefix(ref PawnGenerationRequest request)
        {
            Atavism = false;
            CurrentForcedXenotype = null;

            Pawn mother = Patch_PregnancyUtility_ApplyBirthOutcome.mother;
            if (mother?.genes == null) return;

            GeneticAtavismDef atavismDef = AtavismUtils.TryPickAtavismDef(mother);
            if (atavismDef == null)
                return;
            AtavismGeneInheritanceResult genes = AtavismUtils.GetInheritedGenes(request, mother, atavismDef);

            if (request.ForcedEndogenes == null)
                request.ForcedEndogenes = new List<GeneDef>();

            if (request.ForcedXenogenes == null)
                request.ForcedXenogenes = new List<GeneDef>();


            request.ForcedEndogenes.Clear();
            request.ForcedXenogenes.Clear();

            request.ForcedEndogenes.AddRange(genes.endogenes);
            request.ForcedXenogenes.AddRange(genes.xenogenes);

            request.ForcedXenotype = atavismDef.targetXenotype;
            CurrentForcedXenotype = atavismDef.targetXenotype;
            Atavism = true;
        }
    }
}