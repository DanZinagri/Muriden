using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace AtavismLibraries;

[DefOf]
public static class Atavism_DefOf
{
    public static LetterDef ML_GeneticAtavism;

    static Atavism_DefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(Atavism_DefOf));
    }

    public static LetterDef GetAtavismLetter()
    {
        return ML_GeneticAtavism;
    }
}