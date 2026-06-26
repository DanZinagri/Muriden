using AtavismLibraries;
using Verse;

namespace MuridenLibraries.HarmonyPatches;

[StaticConstructorOnStartup]
public static class HarmonyInit
{
    static HarmonyInit()
    {
        AtavismMod.Harm.PatchAll();
    }
}
