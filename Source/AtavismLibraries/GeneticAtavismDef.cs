using RimWorld;
using System.Collections.Generic;
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

        // Defs are rolled lowest order first, and the first one that passes its
        // chance wins. Defs sharing an order keep their XML order, so a
        // guaranteed fallback only needs to be listed last with chance 1.
        public int order = 0;

        // Position in DefDatabase, stamped once so equal-order defs roll in a
        // stable sequence instead of whatever the database happens to hand back.
        internal int loadIndex;

        public bool Matches(Pawn pawn)
        {
            //pawn borked, or you didn't specify a target.
            if (pawn?.genes == null || targetXenotype == null)
                return false;

            // Invalid def: no condition was provided.
            if (requiredGene == null && sourceXenotype == null)
                return false;

            if (sourceXenotype != null && pawn.genes.Xenotype != sourceXenotype)
                return false;

            if (requiredGene != null && !AtavismUtils.HasActiveGene(pawn, requiredGene))
                return false;

            return true;
        }

        public override IEnumerable<string> ConfigErrors()
        {
            foreach (string error in base.ConfigErrors())
            {
                yield return error;
            }

            if (targetXenotype == null)
            {
                yield return "targetXenotype is null; this def can never fire.";
            }

            if (requiredGene == null && sourceXenotype == null)
            {
                yield return "neither requiredGene nor sourceXenotype was set; this def can never fire.";
            }

            if (chance <= 0f)
            {
                yield return $"chance is {chance}; this def can never fire.";
            }
            else if (chance > 1f)
            {
                yield return $"chance is {chance}, which is above 1. Use 1 for a guaranteed fallback.";
            }
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
