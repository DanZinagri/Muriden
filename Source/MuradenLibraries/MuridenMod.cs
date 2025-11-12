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

    public MuridenMod(ModContentPack content) : base(content)
    {
        Harm = new("DanZinagri.Muriden");
        //Settings = GetSettings<MuridenSettings>(); maybe one day
    }
}