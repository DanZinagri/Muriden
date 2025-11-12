using RimWorld;
using Verse;

namespace MuridenLibraries;

[DefOf]
public static class ML_DefOf
{
    // public static SoundDef VPE_Killskip_Jump_01a;
    //  public static SoundDef VPE_Killskip_Jump_01b;
    //  public static SoundDef VPE_Killskip_Jump_01c;

    //public static EffecterDef VPE_Skip_ExitNoDelayRed;
    public static ThingDef Muriden_JumpingPawn;

    static ML_DefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(ML_DefOf));
    }
}
