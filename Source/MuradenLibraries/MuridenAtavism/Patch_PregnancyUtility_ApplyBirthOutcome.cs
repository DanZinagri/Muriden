using HarmonyLib;
using MuridenLibraries;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace MuridenLibraries
{
    //The purpose of this patch is to save the current birthing mother if she has the right gene
    //The field will be used to determine in the other patch if the baby has a mother with this specific gene or not
    //The reason is because the mother pawn is not a present parameter/field in the other method
    [HarmonyPatch(typeof(PregnancyUtility), "ApplyBirthOutcome")]
    public static class Patch_PregnancyUtility_ApplyBirthOutcome
    {
        public static Pawn mother;

        [HarmonyPrefix]
        public static void Prefix(Pawn geneticMother)
        {
            if (geneticMother == null) return;
            if (AtavismUtils.HasActiveGene(geneticMother, ML_DefOf.Muriden_Resolve) || AtavismUtils.HasActiveGene(geneticMother, ML_DefOf.Choerites_Calm) || AtavismUtils.HasActiveGene(geneticMother, ML_DefOf.FeralMuriden_Resolve) || AtavismUtils.HasActiveGene(geneticMother, ML_DefOf.Artigasen_Might))
            {
                mother = geneticMother;
            }
            return;
        }

        [HarmonyPostfix]
        public static void Postfix(Thing __result)
        {
            if (__result == null)
            {
                if (mother != null) mother = null;
                return;
            }
            if (Patch_PawnGenerator_GeneratePawn.Atavism)
            {
                Pawn baby = __result as Pawn;
                Patch_PawnGenerator_GeneratePawn.Atavism = false;
                ChoiceLetter_GeneticAtavism choiceLetter_baby = (ChoiceLetter_GeneticAtavism)LetterMaker.MakeLetter("AtavismTitle".Translate(baby.genes.Xenotype.label), "AtavismLoc".Translate(mother, baby.genes.Xenotype.label), ML_DefOf.ML_GeneticAtavism, baby);
                choiceLetter_baby.Start();
                Find.LetterStack.ReceiveLetter(choiceLetter_baby);
            }
            if (mother != null)
            {
                mother = null;
            }
            return;
        }
    }
}
