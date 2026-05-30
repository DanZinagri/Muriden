using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace MuridenLibraries;

[DefOf]
public static class ML_DefOf
{
    public static ThingDef Muriden_JumpingPawn;

    public static XenotypeDef Muriden;
    public static XenotypeDef FeralMuriden;
    public static XenotypeDef Choerites;
    public static XenotypeDef Artigasen;

    public static GeneDef Muriden_Resolve;
    public static GeneDef FeralMuriden_Resolve;
    public static GeneDef Choerites_Calm;
    public static GeneDef Artigasen_Might;

    //public static TraitDef Trait;

    public static LetterDef ML_GeneticAtavism;

    static ML_DefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(ML_DefOf));
    }
}
