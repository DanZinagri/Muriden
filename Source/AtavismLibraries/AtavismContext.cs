using RimWorld;
using Verse;

namespace AtavismLibraries
{
    // The birth currently being resolved.
    //
    // Opened by Patch_PregnancyUtility_ApplyBirthOutcome and always
    // closed again by its finalizer, so a throw partway through pawn generation
    // cannot leak the mother onto the next pawn generated anywhere in the game.
    public static class AtavismContext
    {
        private static int resolvingGenesDepth;

        public static Pawn Mother { get; private set; }

        public static Pawn Father { get; private set; }

        public static GeneticAtavismDef Def { get; private set; }

        // True once the baby's generation request has been rewritten.
        public static bool Applied { get; private set; }

        public static bool Active => Def != null;

        public static XenotypeDef TargetXenotype => Def?.targetXenotype;

        // True while a gene set is being worked out.
        //
        // ApplyBirthOutcome resolves its gene list part way through, before it
        // generates the baby, and Better Gene Inheritance generates a throwaway
        // pawn during that resolution. That pawn must not be mistaken for the
        // baby, or it would swallow the atavism and leave the real one plain.
        public static bool ResolvingGenes => resolvingGenesDepth > 0;

        public static void PushResolvingGenes()
        {
            resolvingGenesDepth++;
        }

        public static void PopResolvingGenes()
        {
            if (resolvingGenesDepth > 0)
            {
                resolvingGenesDepth--;
            }
        }

        public static void Open(Pawn mother, Pawn father, GeneticAtavismDef def)
        {
            Mother = mother;
            Father = father;
            Def = def;
            Applied = false;
        }

        // Deliberately leaves resolvingGenesDepth alone: it is balanced by the
        // patch that raised it, and zeroing it here could unbalance that.
        public static void Close()
        {
            Mother = null;
            Father = null;
            Def = null;
            Applied = false;
        }

        // Claims the atavism for the pawn about to be generated, once. Returns
        // false when there is nothing to apply, when the baby has already been
        // generated, or when we are inside gene resolution and so looking at
        // somebody else's throwaway pawn.
        public static bool TryBeginApply(out GeneticAtavismDef def, out Pawn mother, out Pawn father)
        {
            def = null;
            mother = null;
            father = null;

            if (!Active || Applied || ResolvingGenes)
                return false;

            def = Def;
            mother = Mother;
            father = Father;

            // Set before the caller starts building gene lists, so that anything
            // which generates a pawn while doing so cannot claim the atavism too.
            Applied = true;
            return true;
        }
    }
}
