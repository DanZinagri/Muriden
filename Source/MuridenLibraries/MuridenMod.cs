using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace MuridenLibraries;

public class MuridenMod : Mod
{
    public static MuridenSettings Settings;

    public MuridenMod(ModContentPack content) : base(content)
    {
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
        listing.CheckboxLabeled("Muriden.factionPatch".Translate(), ref Settings.factionPatch, "Muriden.factionPatch.Desc".Translate());
        listing.End();
    }
}

public class MuridenSettings : ModSettings
{
    public bool dirtmolePatch = false;
    public bool factionPatch = false;

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref dirtmolePatch, nameof(dirtmolePatch), false);
        Scribe_Values.Look(ref factionPatch, nameof(factionPatch), false);
    }
}