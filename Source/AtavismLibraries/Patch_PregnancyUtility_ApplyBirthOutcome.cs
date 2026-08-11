using HarmonyLib;
using RimWorld;
using Verse;

namespace AtavismLibraries
{
    //This is where an atavism birth is decided. The mother is not a parameter of
    //PawnGenerator.GeneratePawn, so everything we need is worked out here and
    //parked in AtavismContext for the rest of the birth to pick up.
    [HarmonyPatch(typeof(PregnancyUtility), "ApplyBirthOutcome")]
    public static class Patch_PregnancyUtility_ApplyBirthOutcome
    {
        [HarmonyPrefix]
        public static void Prefix(Pawn geneticMother, Pawn father)
        {
            AtavismContext.Close();

            if (geneticMother?.genes == null)
                return;

            GeneticAtavismDef def = AtavismUtils.TryPickAtavismDef(geneticMother);
            if (def == null)
                return;

            // Only the decision is made here. The gene lists are built later, in
            // the GeneratePawn prefix, because the list this birth will actually
            // use is not resolved until part way through the original method - the
            // genes argument is usually null at this point.
            AtavismContext.Open(geneticMother, father, def);
        }

        //Runs after Better Gene Inheritance's own ApplyBirthOutcome postfix, which
        //sits at Priority.Low and reassigns the baby's xenotype to whichever parent
        //scores highest against its gene list. For an atavism birth the target
        //xenotype is the entire point, so re-assert it and only then announce it.
        //Harmless when BGI is absent - it just restates what we already forced.
        [HarmonyPostfix]
        [HarmonyPriority(Priority.VeryLow)]
        [HarmonyAfter(BGICompat.HarmonyId)]
        public static void Postfix(Thing __result, bool preventLetter)
        {
            if (!AtavismContext.Applied)
                return;

            XenotypeDef target = AtavismContext.TargetXenotype;
            Pawn mother = AtavismContext.Mother;

            if (target == null || mother == null || __result is not Pawn baby || baby.genes == null)
                return;

            baby.genes.SetXenotypeDirect(target);
            baby.genes.xenotypeName = null;
            baby.genes.iconDef = null;
            baby.genes.hybrid = false;

            // The faction check mirrors ChoiceLetter_GeneticAtavism.CanShowInLetterStack.
            // Without it, every enemy or visitor birth still built a letter that
            // could never be shown and went straight to the archive.
            if (preventLetter || baby.Faction?.IsPlayer != true)
                return;

            ChoiceLetter_GeneticAtavism letter = (ChoiceLetter_GeneticAtavism)LetterMaker.MakeLetter(
                "AtavismTitle".Translate(target.label),
                "AtavismLoc".Translate(mother, target.label),
                Atavism_DefOf.GetAtavismLetter(),
                baby);
            letter.Start();
            Find.LetterStack.ReceiveLetter(letter);
        }

        //A finalizer runs even when the birth throws, which a postfix does not.
        //Without it a failed generation would leave the mother parked in the
        //context and the next pawn generated anywhere would inherit her atavism.
        [HarmonyFinalizer]
        public static void Finalizer()
        {
            AtavismContext.Close();
        }
    }
}
