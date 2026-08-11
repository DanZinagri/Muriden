using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using Verse;

namespace AtavismLibraries
{
    // Soft link to Better Gene Inheritance.
    // Note that most of the benefit of having BGI installed arrives for free -
    // BGI prefixes <c>PregnancyUtility.GetInheritedGenes</c> at conception, so
    // the gene list handed to ApplyBirthOutcome is already its mix. This bridge
    // only exists for the modes that want a fresh parent mix rather than the
    // stored one.
    [StaticConstructorOnStartup]
    public static class BGICompat
    {
        public const string PackageId = "RedMattis.BetterGeneInheritance";

        /// <summary>BGI's Harmony id, used to order our patches after theirs.</summary>
        public const string HarmonyId = "RedMattis.BGInheritance";

        private static readonly MethodInfo getChildGenes;
        private static bool failed;

        /// <summary>True when BGI is loaded and its external API was found.</summary>
        public static bool Active => getChildGenes != null;

        static BGICompat()
        {
            if (!ModsConfig.IsActive(PackageId))
                return;

            getChildGenes = AccessTools.Method(
                "BGInheritance.External:GetChildGenes",
                new[] { typeof(Pawn), typeof(Pawn) });

            if (getChildGenes == null)
            {
                Log.Warning("[Atavism] Better Gene Inheritance is loaded but " +
                    "BGInheritance.External:GetChildGenes could not be found. " +
                    "Falling back to built-in inheritance.");
            }
        }

        // Asks BGI for a child gene set. Returns null when BGI is unavailable,
        // disabled in settings, or has no answer, in which case the caller
        // should use its own handling.
        public static List<GeneDef> TryGetChildGenes(Pawn father, Pawn mother)
        {
            if (!Active || failed)
                return null;

            if (!AtavismMod.Settings.useBetterGeneInheritance)
                return null;

            if (father == null && mother == null)
                return null;

            // GetChildGenes generates a throwaway pawn internally, and it reaches
            // BGI's mixing directly rather than through PregnancyUtility, so the
            // GetInheritedGenes patch does not cover this call. Flag it here too,
            // or our own GeneratePawn prefix would force an atavism onto that pawn
            // and recurse straight back into here.
            AtavismContext.PushResolvingGenes();
            try
            {
                var genes = getChildGenes.Invoke(null, new object[] { father, mother }) as List<GeneDef>;

                // BGI's own prefix returns an empty list when it decides to defer
                // to vanilla. Treat that as "no answer", not "no genes".
                return genes != null && genes.Count > 0 ? genes : null;
            }
            catch (Exception e)
            {
                failed = true;
                Log.Error("[Atavism] Better Gene Inheritance threw while building child genes. " +
                    "Falling back to built-in inheritance for the rest of this session.\n" + e);
                return null;
            }
            finally
            {
                AtavismContext.PopResolvingGenes();
            }
        }
    }
}
