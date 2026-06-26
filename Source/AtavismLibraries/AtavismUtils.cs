using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;


namespace AtavismLibraries
{
    public static class AtavismUtils
    {

        public static List<GeneticAtavismDef> GetMatchingAtavismDefs(Pawn pawn)
        {
            if (pawn?.genes == null)
                return new List<GeneticAtavismDef>();

            return DefDatabase<GeneticAtavismDef>
                .AllDefsListForReading
                .Where(def => def.Matches(pawn))
                .ToList();
        }

        public static GeneticAtavismDef TryPickAtavismDef(Pawn pawn)
        {
            List<GeneticAtavismDef> matches = GetMatchingAtavismDefs(pawn);

            foreach (GeneticAtavismDef def in matches)
            {
                if (Rand.Chance(def.chance))
                {
                    return def;
                }
            }

            return null;
        }


        //A bunch of utility methods to avoid code duplication
        public static bool HasActiveGene(Pawn pawn, GeneDef geneDef)
        {
            if (pawn.genes is null || geneDef is null)
            {
                return false;
            }
            bool? isGeneActive = pawn.genes.GetGene(geneDef)?.Active;
            if (isGeneActive == true)
            {
                return true;
            }
            return false;
        }

        public static List<GeneDef> ClearNonCosmeticGeneBaby(List<GeneDef> babyGenes)
        {
            if (babyGenes == null) return new List<GeneDef>();
            List<GeneDef> filteredGenes = new List<GeneDef>();
            for (int i = 0; i < babyGenes.Count; i++)
            {
                if (babyGenes[i].endogeneCategory == EndogeneCategory.Melanin || babyGenes[i].endogeneCategory == EndogeneCategory.HairColor)
                {
                    filteredGenes.Add(babyGenes[i]);
                }
            }
            return filteredGenes;
        }
    }
}
