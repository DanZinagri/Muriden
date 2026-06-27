using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AtavismLibraries
{
    public class GeneticAtavismDef : Def
    {
        public GeneDef requiredGene;
        public XenotypeDef sourceXenotype;
        public XenotypeDef targetXenotype;
        public AtavismInheritanceMode InheritanceMode = AtavismInheritanceMode.none;
        public float chance = 1f;

        public bool Matches(Pawn pawn)
        {
            //pawn borked, or you didn't specify a target.
            if (pawn?.genes == null || targetXenotype == null)
                return false;

            // Invalid def: no condition was provided.
            if (requiredGene == null && sourceXenotype == null)
                return false;

            bool geneMatches = requiredGene == null ||
                       AtavismUtils.HasActiveGene(pawn, requiredGene);

            bool xenotypeMatches = sourceXenotype == null ||
                                   pawn.genes.Xenotype == sourceXenotype;

            return geneMatches && xenotypeMatches;
        }
    }

    public enum AtavismInheritanceMode
    {
        none,
        excess,
        endo,
        xeno,
        all
    }
}
