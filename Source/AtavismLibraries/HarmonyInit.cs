using Verse;

namespace AtavismLibraries;

[StaticConstructorOnStartup]
public static class HarmonyInit
{
    static HarmonyInit()
    {
        AtavismMod.Harm.PatchAll();
    }
}
