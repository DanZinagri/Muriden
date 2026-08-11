using HarmonyLib;
using RimWorld;
using System;
using Verse;

namespace AtavismLibraries
{
    //ApplyBirthOutcome resolves its gene list mid-method, before it generates the
    //baby. Better Gene Inheritance prefixes this method and generates a throwaway
    //pawn to do its mixing, which would otherwise reach our GeneratePawn patch
    //and claim the atavism, leaving the actual baby unchanged.
    //
    //Flagging the window keeps any such pawn - BGI's or another mod's - invisible
    //to us. The two-argument overload delegates to this one, so patching here
    //covers both.
    [HarmonyPatch(typeof(PregnancyUtility), "GetInheritedGenes",
        new Type[] { typeof(Pawn), typeof(Pawn), typeof(bool) },
        new ArgumentType[] { ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Out })]
    public static class Patch_PregnancyUtility_GetInheritedGenes
    {
        //Priority.First so the flag is up before Better Gene Inheritance's own
        //prefix runs - it returns false and skips the original, which would skip
        //a lower-priority prefix of ours as well.
        [HarmonyPrefix]
        [HarmonyPriority(Priority.First)]
        public static void Prefix()
        {
            AtavismContext.PushResolvingGenes();
        }

        //A finalizer rather than a postfix: it runs even when another mod's prefix
        //skips the original or something throws, so the flag can never stick.
        [HarmonyFinalizer]
        public static void Finalizer()
        {
            AtavismContext.PopResolvingGenes();
        }
    }
}
