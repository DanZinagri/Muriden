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
    //Patch to apply a chance for the pawn to be born as an alternate xenotype on a chance.
    [HarmonyPatch(typeof(PawnGenerator), "GeneratePawn", new Type[] { typeof(PawnGenerationRequest) })]
    public static class Patch_PawnGenerator_GeneratePawn
    {
        public static bool Atavism = false;

        [HarmonyPrefix]
        public static void Prefix(ref PawnGenerationRequest request)
        {
            if (Patch_PregnancyUtility_ApplyBirthOutcome.mother == null) return;
            Pawn mother = Patch_PregnancyUtility_ApplyBirthOutcome.mother;
            bool isBabyAtavism = false;
            //we're gonna want 
            if (AtavismUtils.IsMuriden(mother) || AtavismUtils.IsFeralMuriden(mother))
            {
                //grab our xenotype info commenting this out because i'm gonna forget this if i need these later
                //XenotypeDef xenotypeA = DefDatabase<XenotypeDef>.GetNamedSilentFail("Muriden");
                //XenotypeDef xenotypeB = DefDatabase<XenotypeDef>.GetNamedSilentFail("FeralMuriden");
                //xenotypeA ??= ML_DefOf.Muriden;
                //xenotypeB ??= ML_DefOf.FeralMuriden;
                XenotypeDef xenotypeA = ML_DefOf.Muriden;
                XenotypeDef xenotypeB = ML_DefOf.FeralMuriden;

                //being lazy for now and hard-coding this; might add a settng later
                if (AtavismUtils.IsFeralMuriden(mother))
                {
                    //the logic is once we're already feral muriden the chance of basically re-evolving into a muriden is gonna be low; thus high atavism chance)
                    isBabyAtavism = Rand.Chance(0.96f);
                }
                else
                {
                    //We want this to be a lower chance.
                    isBabyAtavism = Rand.Chance(0.05f);
                }

                //tldr we still allow cosmetic genes to be inherented, but all other genes are forced
                List<GeneDef> babyInheritedGenes = request.ForcedEndogenes;
                List<GeneDef> babyCosmeticGenes = AtavismUtils.ClearNonCosmeticGeneBaby(babyInheritedGenes);
                request.ForcedEndogenes.Clear();
                request.ForcedEndogenes = babyCosmeticGenes;

                request.ForcedXenotype = isBabyAtavism ? xenotypeB : xenotypeA;

                if (isBabyAtavism)
                {
                    Atavism = true;
                }
            }
            else if (AtavismUtils.IsChoerites(mother) || AtavismUtils.IsArtigasen(mother))
            {
                XenotypeDef xenotypeA = ML_DefOf.Choerites;
                XenotypeDef xenotypeB = ML_DefOf.Artigasen;

                //being lazy for now and hard-coding this; might add a settng later
                if (AtavismUtils.IsArtigasen(mother))
                {
                    isBabyAtavism = Rand.Chance(0.90f);
                }
                else
                {
                    isBabyAtavism = Rand.Chance(0.05f);
                }

                //tldr we still allow cosmetic genes to be inherented, but all other genes are forced
                List<GeneDef> babyInheritedGenes = request.ForcedEndogenes;
                List<GeneDef> babyCosmeticGenes = AtavismUtils.ClearNonCosmeticGeneBaby(babyInheritedGenes);
                request.ForcedEndogenes.Clear();
                request.ForcedEndogenes = babyCosmeticGenes;

                request.ForcedXenotype = isBabyAtavism ? xenotypeB : xenotypeA;

                if (isBabyAtavism)
                {
                    Atavism = true;
                }
            }else if(AtavismUtils.isSciurus(mother))
            {
                XenotypeDef xenotypeA = ML_DefOf.Sciurus;
                if (AtavismUtils.isSciurus(mother))
                {
                    isBabyAtavism = false;
                }

                List<GeneDef> babyInheritedGenes = request.ForcedEndogenes;
                List<GeneDef> babyCosmeticGenes = AtavismUtils.ClearNonCosmeticGeneBaby(babyInheritedGenes);
                request.ForcedEndogenes.Clear();
                request.ForcedEndogenes = babyCosmeticGenes;

                request.ForcedXenotype = xenotypeA;//isBabyAtavism ? xenotypeB : xenotypeA;

                if (isBabyAtavism)
                {
                    Atavism = true;
                }
            }
            else if (AtavismUtils.isCastoran(mother))
            {
                XenotypeDef xenotypeA = ML_DefOf.Castoran;
                if (AtavismUtils.isCastoran(mother))
                {
                    isBabyAtavism = false;
                }

                List<GeneDef> babyInheritedGenes = request.ForcedEndogenes;
                List<GeneDef> babyCosmeticGenes = AtavismUtils.ClearNonCosmeticGeneBaby(babyInheritedGenes);
                request.ForcedEndogenes.Clear();
                request.ForcedEndogenes = babyCosmeticGenes;

                request.ForcedXenotype = xenotypeA;//isBabyAtavism ? xenotypeB : xenotypeA;

                if (isBabyAtavism)
                {
                    Atavism = true;
                }
            }
            return;
        }
    }
}