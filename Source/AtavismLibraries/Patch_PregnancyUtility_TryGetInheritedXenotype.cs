using HarmonyLib;
using RimWorld;
using Verse;

namespace AtavismLibraries
{
    //By default the game decides the parents will produce a "Hybrid", which would
    //override the xenotype we forced. The context stays open for the whole birth,
    //so this covers the call whether it comes from ApplyBirthOutcome directly or
    //from inside the baby's own generation.
    [HarmonyPatch(typeof(PregnancyUtility), "TryGetInheritedXenotype")]
    public static class Patch_PregnancyUtility_TryGetInheritedXenotype
    {
        [HarmonyPostfix]
        public static void Postfix(ref XenotypeDef xenotype, ref bool __result)
        {
            XenotypeDef target = AtavismContext.TargetXenotype;
            if (target == null)
                return;

            xenotype = target;
            __result = true;
        }
    }
}
