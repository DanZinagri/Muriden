using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
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
            if (pawn?.genes == null) return false;
            if (requiredGene == null || sourceXenotype == null || targetXenotype == null) return false;

            return pawn.genes.Xenotype == sourceXenotype
                && AtavismUtils.HasActiveGene(pawn, requiredGene);
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
