using MuridenLibraries;
using Verse;

namespace VanillaPsycastsExpanded.HarmonyPatches;

[StaticConstructorOnStartup]
public static class HarmonyInit
{
    static HarmonyInit()
    {
        MuridenMod.Harm.PatchAll();
    }
}
