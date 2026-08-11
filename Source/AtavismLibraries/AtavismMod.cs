using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace AtavismLibraries;

public class AtavismMod : Mod
{
    public static Harmony Harm;
    public static AtavismSettings Settings;

    public AtavismMod(ModContentPack content) : base(content)
    {
        Harm = new Harmony("DanZinagri.Atavism");
        Settings = GetSettings<AtavismSettings>();
    }

    public override string SettingsCategory()
    {
        return "Atavism_SettingsCategory".Translate();
    }

    public override void DoSettingsWindowContents(Rect inRect)
    {
        Listing_Standard listing = new();
        listing.Begin(inRect);

        if (BGICompat.Active)
        {
            listing.CheckboxLabeled(
                "Atavism_UseBGI".Translate(),
                ref Settings.useBetterGeneInheritance,
                "Atavism_UseBGIDesc".Translate());
        }
        else
        {
            GUI.color = Color.gray;
            listing.Label("Atavism_BGINotLoaded".Translate());
            GUI.color = Color.white;
        }

        listing.End();
        base.DoSettingsWindowContents(inRect);
    }
}
