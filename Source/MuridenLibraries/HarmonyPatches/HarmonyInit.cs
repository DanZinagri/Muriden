using MuridenLibraries;
using Verse;

namespace MuridenLibraries.HarmonyPatches;

[StaticConstructorOnStartup]
public static class HarmonyInit
{
    static HarmonyInit()
    {
        MuridenMod.Harm.PatchAll();
    }
}
