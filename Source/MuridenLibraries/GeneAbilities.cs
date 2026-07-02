using System.Collections.Generic;
using RimWorld;
using Verse;
using VEF.Abilities;
using AbilityDef = VEF.Abilities.AbilityDef;

namespace MuridenLibraries
{
    public class GeneExtension_VEFAbilities : DefModExtension
    {
        public List<AbilityDef> abilityDefs = new();
    }

    public class Gene_GiveVEFAbilities : Gene
    {
        public override void PostAdd()
        {
            base.PostAdd();

            GeneExtension_VEFAbilities ext = def.GetModExtension<GeneExtension_VEFAbilities>();
            if (ext?.abilityDefs == null || pawn == null)
                return;

            CompAbilities comp = pawn.TryGetComp<CompAbilities>();
            if (comp == null)
                return;

            foreach (AbilityDef ability in ext.abilityDefs)
            {
                if (ability != null && !comp.HasAbility(ability))
                {
                    comp.GiveAbility(ability);
                }
            }
        }
    }
}