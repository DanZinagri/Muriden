using System;
using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace MuridenLibraries;

public class MuridenMod : Mod
{
    public static Harmony Harm;
    public static MuridenSettings Settings;

    public MuridenMod(ModContentPack content) : base(content)
    {
        Harm = new("DanZinagri.Muriden");
        Settings = GetSettings<MuridenSettings>();

        LongEventHandler.ExecuteWhenFinished(ApplySettings);
    }

    public override string SettingsCategory() => "MuridenTitle".Translate();

    public override void WriteSettings()
    {
        base.WriteSettings();
        //ApplySettings();
    }

    private void ApplySettings()
    {
        //HediffDefOf.PsychicAmplifier.maxSeverity = Settings.maxLevel;
    }

    public override void DoSettingsWindowContents(Rect inRect)
    {
        base.DoSettingsWindowContents(inRect);
        Listing_Standard listing = new();
        listing.Begin(inRect);
        listing.CheckboxLabeled("Muriden.dirtmolePatch".Translate(), ref Settings.dirtmolePatch, "Muriden.dirtmolePatch.Desc".Translate());
        listing.End();
    }
}

public class MuridenSettings : ModSettings
{
    public bool dirtmolePatch = true;

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref dirtmolePatch, nameof(dirtmolePatch), true);
    }
}