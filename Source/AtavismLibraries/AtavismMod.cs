using HarmonyLib;
using Verse;

namespace AtavismLibraries;

public class AtavismMod : Mod
{
    public static Harmony Harm;

    public AtavismMod(ModContentPack content) : base(content)
    {
        Harm = new Harmony("DanZinagri.Atavism");
    }
}