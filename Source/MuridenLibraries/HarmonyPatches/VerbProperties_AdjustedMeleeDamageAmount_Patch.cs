namespace MuridenLibraries
{
    using HarmonyLib;
    using RimWorld;
    using System;
    using Verse;

    [HarmonyPatch(typeof(VerbProperties), "AdjustedMeleeDamageAmount", new Type[] { typeof(Verb), typeof(Pawn) })]
    public static class VerbProperties_MundaneMeleeDamageAmount_Patch
    {
        public static bool multiplyMundaneByPawnMeleeSkill;
        private static void Postfix(ref float __result, Verb ownerVerb, Pawn attacker)
        {
            if (attacker != null && multiplyMundaneByPawnMeleeSkill)
            {
                __result *= (attacker.skills.GetSkill(SkillDefOf.Melee).Level / 10f);
            }
        }
    }
}