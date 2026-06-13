using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using UnityEngine;
using RimWorld;
using UnityEngine.Analytics;


namespace MuridenLibraries
{
    public static class AtavismUtils
    {
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

        public static List<Gene> GetAllGenesFromMother(Pawn mother)
        {
            if (mother == null) return new List<Gene>();
            List<Gene> motherGenes = new List<Gene>();
            motherGenes.AddRange(mother.genes.Xenogenes);
            motherGenes.AddRange(mother.genes.Endogenes);
            return motherGenes;
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

        public static bool IsMuriden(Pawn pawn)
        {
            return pawn.genes.Xenotype == ML_DefOf.Muriden;
        }

        public static bool IsFeralMuriden(Pawn pawn)
        {
            return pawn.genes.Xenotype == ML_DefOf.FeralMuriden;
        }

        public static bool IsChoerites(Pawn pawn)
        {
            return pawn.genes.Xenotype == ML_DefOf.Choerites;
        }

        public static bool IsArtigasen(Pawn pawn)
        {
            return pawn.genes.Xenotype == ML_DefOf.Artigasen;
        }

        public static bool isSciurus(Pawn pawn)
        {
            return pawn.genes.Xenotype == ML_DefOf.Sciurus;
        }

        public static bool isCastoran(Pawn pawn)
        {
            return pawn.genes.Xenotype == ML_DefOf.Castoran;
        }
    }
}
