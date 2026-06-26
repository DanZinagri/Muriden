using RimWorld;
using Verse;

namespace AtavismLibraries
{
    public class GeneticAtavismDef : Def
    {
        public GeneDef requiredGene;
        public XenotypeDef sourceXenotype;
        public XenotypeDef targetXenotype;
        public float chance = 1f;

        public bool Matches(Pawn pawn)
        {
            if (pawn?.genes == null) return false;
            if (requiredGene == null || sourceXenotype == null || targetXenotype == null) return false;

            return pawn.genes.Xenotype == sourceXenotype
                && AtavismUtils.HasActiveGene(pawn, requiredGene);
        }
    }
}