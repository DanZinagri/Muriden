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

    //base muriden
    public static XenotypeDef Muriden;
    public static GeneDef Muriden_Resolve;
    //muriden atavism
    public static XenotypeDef FeralMuriden;
    public static GeneDef FeralMuriden_Resolve;
    //base Choerites
    public static XenotypeDef Choerites;
    public static GeneDef Choerites_Calm;
    //Choerites atavism
    public static XenotypeDef Artigasen;
    public static GeneDef Artigasen_Might;
    //base Sciurus
    public static XenotypeDef Sciurus;
    public static GeneDef Sciurus_Alacrity;
    //sciurus atavism
    //base Castoran
    public static XenotypeDef Castoran;
    public static GeneDef Castoran_Ingenuity;
    //Castoran atavism

    static ML_DefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(ML_DefOf));
    }
}
