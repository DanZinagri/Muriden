using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;


namespace AtavismLibraries
{
    public static class AtavismUtils
    {
        private static List<GeneticAtavismDef> orderedDefs;

        // DefDatabase order is mod load order, then file discovery order, then
        // document order. Inside a single file that means XML order is preserved,
        // but it is an implementation detail and it breaks down the moment defs
        // arrive from more than one file or mod - hence the explicit field. Defs
        // that leave <c>order</c> unset all sit at 0 and so keep their XML order.
        public static List<GeneticAtavismDef> OrderedDefs
        {
            get
            {
                if (orderedDefs == null)
                {
                    List<GeneticAtavismDef> all = DefDatabase<GeneticAtavismDef>.AllDefsListForReading;
                    for (int i = 0; i < all.Count; i++)
                    {
                        all[i].loadIndex = i;
                    }

                    orderedDefs = all
                        .OrderBy(def => def.order)
                        .ThenBy(def => def.loadIndex)
                        .ToList();
                }
                return orderedDefs;
            }
        }

        public static List<GeneticAtavismDef> GetMatchingAtavismDefs(Pawn pawn)
        {
            if (pawn?.genes == null)
                return new List<GeneticAtavismDef>();

            List<GeneticAtavismDef> defs = OrderedDefs;
            List<GeneticAtavismDef> result = new List<GeneticAtavismDef>(defs.Count);
            for (int i = 0; i < defs.Count; i++)
            {
                GeneticAtavismDef def = defs[i];
                if (def.Matches(pawn))
                {
                    result.Add(def);
                }
            }
            return result;
        }
        // Rolls each matching def in <see cref="OrderedDefs"/> order and returns
        // the first one that passes its own chance, so a Muriden with both a
        // Feral and a Castoran def rolls Feral first and only falls through to
        // Castoran if Feral missed. Returns null when every def missed, which
        // leaves the birth on the vanilla path. A def with chance 1 listed last
        // therefore acts as a guaranteed fallback.
        public static GeneticAtavismDef TryPickAtavismDef(Pawn pawn)
        {
            if (pawn?.genes == null)
                return null;

            List<GeneticAtavismDef> defs = OrderedDefs;
            for (int i = 0; i < defs.Count; i++)
            {
                GeneticAtavismDef def = defs[i];
                if (def.Matches(pawn) && Rand.Chance(def.chance))
                {
                    return def;
                }
            }
            return null;
        }


        //A bunch of utility methods to avoid code duplication
        public static bool HasActiveGene(Pawn pawn, GeneDef geneDef)
        {
            if (pawn?.genes is null || geneDef is null)
            {
                return false;
            }
            return pawn.genes.GetGene(geneDef)?.Active == true;
        }

        // Genes the mother carries beyond her xenotype's normal loadout. These
        // are hers alone, so they are worth passing on even when the baby is
        // switching xenotype entirely.
        public static List<GeneDef> GetExcessEndogenes(Pawn mother)
        {
            if (mother?.genes == null)
                return new List<GeneDef>();

            List<Gene> endogenes = mother.genes.Endogenes;
            List<GeneDef> excessGenes = new List<GeneDef>(endogenes.Count);

            if (mother.genes.Xenotype == null)
            {
                for (int i = 0; i < endogenes.Count; i++)
                {
                    excessGenes.Add(endogenes[i].def);
                }
                return excessGenes;
            }

            HashSet<GeneDef> sourceGenes = new HashSet<GeneDef>(mother.genes.Xenotype.AllGenes);
            for (int i = 0; i < endogenes.Count; i++)
            {
                Gene gene = endogenes[i];
                if (!sourceGenes.Contains(gene.def))
                {
                    excessGenes.Add(gene.def);
                }
            }

            return excessGenes;
        }

        // Skin and hair colour only, so the baby still looks related.
        public static List<GeneDef> GetCosmeticGenes(List<GeneDef> genes)
        {
            if (genes == null)
                return new List<GeneDef>();

            List<GeneDef> filteredGenes = new List<GeneDef>(genes.Count);
            for (int i = 0; i < genes.Count; i++)
            {
                GeneDef gene = genes[i];
                if (gene == null)
                    continue;

                if (gene.endogeneCategory == EndogeneCategory.Melanin || gene.endogeneCategory == EndogeneCategory.HairColor)
                {
                    filteredGenes.Add(gene);
                }
            }
            return filteredGenes;
        }

        public static List<GeneDef> GetMotherXenogenes(Pawn mother)
        {
            if (mother?.genes == null)
                return new List<GeneDef>();

            List<Gene> xenogenes = mother.genes.Xenogenes;
            List<GeneDef> result = new List<GeneDef>(xenogenes.Count);
            for (int i = 0; i < xenogenes.Count; i++)
            {
                result.Add(xenogenes[i].def);
            }
            return result;
        }


        public class AtavismGeneInheritanceResult
        {
            public List<GeneDef> endogenes = new List<GeneDef>();
            public List<GeneDef> xenogenes = new List<GeneDef>();
        }

        // Works out the gene lists for an atavism birth.
        public static AtavismGeneInheritanceResult BuildInheritedGenes(
            List<GeneDef> inheritedGenes,
            Pawn mother,
            Pawn father,
            GeneticAtavismDef def)
        {
            AtavismGeneInheritanceResult result = new AtavismGeneInheritanceResult();

            if (def?.targetXenotype == null)
                return result;

            // A non-inheritable target belongs on the xenogene side, otherwise the
            // baby would pass it down as if it were born to it.
            if (def.targetXenotype.inheritable)
            {
                result.endogenes.AddRange(def.targetXenotype.AllGenes);
            }
            else
            {
                result.xenogenes.AddRange(def.targetXenotype.AllGenes);
            }

            List<GeneDef> parentGenes = inheritedGenes ?? new List<GeneDef>();

            switch (def.InheritanceMode)
            {
                case AtavismInheritanceMode.none: //original logic
                    result.endogenes.AddRange(GetCosmeticGenes(parentGenes));
                    break;

                case AtavismInheritanceMode.excess: //we can still pass SOMETHING on
                    result.endogenes.AddRange(GetExcessEndogenes(mother));
                    break;

                case AtavismInheritanceMode.endo: //vanilla-ish birth on top of the target xenotype
                    result.endogenes.AddRange(ParentGenesFor(parentGenes, mother, father));
                    break;

                case AtavismInheritanceMode.xeno: //like none, but the mother's xenogenes carry over
                    result.endogenes.AddRange(GetCosmeticGenes(parentGenes));
                    result.xenogenes.AddRange(GetMotherXenogenes(mother));
                    break;

                case AtavismInheritanceMode.all:
                    result.endogenes.AddRange(ParentGenesFor(parentGenes, mother, father));
                    result.xenogenes.AddRange(GetMotherXenogenes(mother));
                    break;
            }

            Deduplicate(result);
            return result;
        }

        // The parent contribution for the modes that stack a full gene set on top
        // of the target xenotype. Prefers a fresh Better Gene Inheritance mix and
        // falls back to the pregnancy's own list.
        private static List<GeneDef> ParentGenesFor(List<GeneDef> parentGenes, Pawn mother, Pawn father)
        {
            return BGICompat.TryGetChildGenes(father, mother) ?? parentGenes;
        }

        // A gene can only be held once, and never as both an endogene and a
        // xenogene. Piling the target xenotype on top of a parent gene set makes
        // overlap the norm rather than the exception.
        private static void Deduplicate(AtavismGeneInheritanceResult result)
        {
            HashSet<GeneDef> endogeneSet = new HashSet<GeneDef>();
            List<GeneDef> uniqueEndogenes = new List<GeneDef>(result.endogenes.Count);
            for (int i = 0; i < result.endogenes.Count; i++)
            {
                GeneDef gene = result.endogenes[i];
                if (gene != null && endogeneSet.Add(gene))
                {
                    uniqueEndogenes.Add(gene);
                }
            }
            result.endogenes = uniqueEndogenes;

            HashSet<GeneDef> xenogeneSet = new HashSet<GeneDef>();
            List<GeneDef> uniqueXenogenes = new List<GeneDef>(result.xenogenes.Count);
            for (int i = 0; i < result.xenogenes.Count; i++)
            {
                GeneDef gene = result.xenogenes[i];
                if (gene != null && !endogeneSet.Contains(gene) && xenogeneSet.Add(gene))
                {
                    uniqueXenogenes.Add(gene);
                }
            }
            result.xenogenes = uniqueXenogenes;
        }
    }
}
