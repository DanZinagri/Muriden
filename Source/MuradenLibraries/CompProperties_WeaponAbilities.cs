using VEF.Abilities;
using Verse;

namespace MuridenLibraries
{
    public class CompProperties_WeaponAbilities : CompProperties
    {
        // This is the field you'll set in XML using the ability's defName
        public AbilityDef ability;

        public CompProperties_WeaponAbilities()
        {
            compClass = typeof(WeaponAbilities);
        }
    }
}
