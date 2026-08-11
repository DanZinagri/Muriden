using Verse;

namespace AtavismLibraries;

public class AtavismSettings : ModSettings
{
    public const bool UseBetterGeneInheritanceDefault = true;

    // Whether the inheritance modes that stack a full parent gene set on the
    // target xenotype ask Better Gene Inheritance for a fresh mix. Has no effect
    // when that mod is not loaded.
    public bool useBetterGeneInheritance = UseBetterGeneInheritanceDefault;

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref useBetterGeneInheritance, "useBetterGeneInheritance", UseBetterGeneInheritanceDefault);
    }
}
