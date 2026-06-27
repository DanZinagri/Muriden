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

        public static List<GeneDef> GetExcessEndogenes(Pawn mother)
        {
            List<GeneDef> excessGenes = new();

            if (mother?.genes == null || mother.genes.Xenotype == null)
                return excessGenes;

            // The genes that every pawn of this xenotype normally has.
            HashSet<GeneDef> sourceGenes = new(
                mother.genes.Xenotype.AllGenes
            );

            foreach (Gene gene in mother.genes.Endogenes)
            {
                // Skip genes that are part of the normal xenotype.
                if (sourceGenes.Contains(gene.def))
                    continue;

                excessGenes.Add(gene.def);
            }

            return excessGenes;
        }

        public static List<GeneDef> GetCosmeticGeneBaby(List<GeneDef> babyGenes)
        {
            if (babyGenes == null) return new List<GeneDef>();
            List<GeneDef> filteredGenes = new();
            foreach(GeneDef gene in babyGenes)
            {
                if (gene.endogeneCategory == EndogeneCategory.Melanin || gene.endogeneCategory == EndogeneCategory.HairColor)
                {
                    filteredGenes.Add(gene);
                }
            }
            return filteredGenes;
        }

        public static List<GeneDef> MergeEndogenes(Pawn mother, GeneticAtavismDef def)
        {
            List<GeneDef> genelist = new();

            if (mother?.genes == null || mother.genes.Xenotype == null)
                return genelist;

            genelist = mother.genes.Xenotype.AllGenes.ToList();

            if (def.targetXenotype == null)
                return genelist;

            foreach (GeneDef gene in def.targetXenotype.AllGenes)
            {
                if (gene != null)
                {
                    genelist.Add(gene);
                }
            }
            return genelist;
        }

        public static List<GeneDef> GetMotherXenogenes(Pawn mother)
        {
            if (mother?.genes == null || mother.genes.Xenotype == null)
                return new List<GeneDef>();

            return mother.genes.Xenogenes.Select(gene => gene.def)
        .ToList();
        }


        public class AtavismGeneInheritanceResult
        {
            public List<GeneDef> endogenes = new List<GeneDef>();
            public List<GeneDef> xenogenes = new List<GeneDef>();
        }

        public static AtavismGeneInheritanceResult GetInheritedGenes(PawnGenerationRequest request, Pawn mother, GeneticAtavismDef def)
        {
            AtavismGeneInheritanceResult result = new AtavismGeneInheritanceResult();

            if (def?.targetXenotype == null)
                return result;

            if (def.targetXenotype.inheritable == true)
            {
                result.endogenes.AddRange(def.targetXenotype.AllGenes);
            }
            else
            {
                result.xenogenes.AddRange(def.targetXenotype.AllGenes);
            }
                

            switch (def.InheritanceMode)
            {
                case AtavismInheritanceMode.none: //original logic
                    result.endogenes.AddRange(GetCosmeticGeneBaby(request.ForcedEndogenes));
                    break;

                case AtavismInheritanceMode.excess: //we can still pass SOMETHING on
                    result.endogenes.AddRange(GetExcessEndogenes(mother));
                    break;

                case AtavismInheritanceMode.endo: //I'd say this was a vanilla birth, but you're gonna have a horrible abomination with BOTH SETS.
                    result.endogenes.AddRange(MergeEndogenes(mother, def));
                    break;

                case AtavismInheritanceMode.xeno: //So we're gonna basically do like we did in none, but pass along any xenogenes.
                    result.endogenes.AddRange(GetCosmeticGeneBaby(request.ForcedEndogenes));
                    result.xenogenes.AddRange(GetMotherXenogenes(mother));
                    break;

                case AtavismInheritanceMode.all:
                    result.endogenes.AddRange(request.ForcedEndogenes);
                    result.xenogenes.AddRange(GetMotherXenogenes(mother));
                    break;
            }

            return result;
        }
    }
}
