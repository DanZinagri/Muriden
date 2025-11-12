using RimWorld;
using System.Collections.Generic;
using System.Linq;
using VEF.Abilities;
using VEF.Graphics;
using VEF.Utils;
using Verse;
using AbilityDef = VEF.Abilities.AbilityDef;

namespace MuridenLibraries
{
    public class CompAbilitiesWeapon : ThingComp
    {
        // NEW: list of abilities this weapon can grant
        public List<AbilityDef> abilities;

        // Track which abilities THIS weapon actually granted on equip (so we can safely remove only those)
        private List<AbilityDef> granted = new();

        // Legacy single-ability field kept for migration
        public AbilityDef ability;

        public CompProperties_AbilitiesWeapon Props => (CompProperties_AbilitiesWeapon)this.props;

        public override void PostExposeData()
        {
            base.PostExposeData();

            // Save/load new fields
            Scribe_Collections.Look(ref abilities, nameof(abilities), LookMode.Def);
            Scribe_Collections.Look(ref granted, nameof(granted), LookMode.Def);

            // Legacy: load the old single field if present (from older saves)
            Scribe_Defs.Look(ref ability, nameof(ability));

            // Migration: if old single field exists and new list is empty, migrate it
            if (abilities == null) abilities = new List<AbilityDef>();
            if (ability != null && abilities.Count == 0)
            {
                abilities.Add(ability);
                ability = null; // clear legacy after migration
            }
        }

        public override void Notify_Equipped(Pawn pawn)
        {
            base.Notify_Equipped(pawn);
            TryGiveAbility(pawn);
        }

        public void TryGiveAbility(Pawn pawn)
        {
            if (abilities == null || abilities.Count == 0) return;

            var comp = pawn.GetComp<CompAbilities>();
            if (comp == null) return;

            // Ensure list exists, and don't duplicate grants across re-equips without unequip
            if (granted == null) granted = new List<AbilityDef>();

            foreach (var def in abilities)
            {
                if (def == null) continue;

                // Only grant if pawn doesn't already have it; record that WE granted it
                if (!comp.HasAbility(def))
                {
                    comp.GiveAbility(def);
                    if (!granted.Contains(def))
                        granted.Add(def);
                }
            }
        }

        public override void Notify_Unequipped(Pawn pawn)
        {
            base.Notify_Unequipped(pawn);

            if (granted == null || granted.Count == 0) return;

            var comp = pawn.GetComp<CompAbilities>();
            if (comp != null && comp.LearnedAbilities != null)
            {
                // Remove only what this weapon granted
                foreach (var def in granted)
                {
                    if (def == null) continue;
                    comp.LearnedAbilities.RemoveAll(ab => ab.def == def);
                }
            }

            // Reset our record for next time this weapon is equipped
            granted.Clear();
        }

        public override void PostPostMake()
        {
            base.PostPostMake();

            // Populate from XML on creation, but don't clobber a save-loaded list
            if ((abilities == null || abilities.Count == 0) && Props != null)
            {
                abilities = new List<AbilityDef>();

                if (Props.abilityDefs != null && Props.abilityDefs.Count > 0)
                {
                    abilities.AddRange(Props.abilityDefs.Where(a => a != null));
                }
                else if (Props.abilityDef != null) // Legacy XML support
                {
                    abilities.Add(Props.abilityDef);
                }
            }
        }

        public override IEnumerable<StatDrawEntry> SpecialDisplayStats()
        {
            if (abilities != null && abilities.Count > 0)
            {
                var labels = abilities.Where(a => a != null).Select(a => a.LabelCap).ToArray();
                if (labels.Length > 0)
                {
                    yield return new StatDrawEntry(StatCategoryDefOf.Weapon_Melee,"VREA.GivesAbility".Translate(),string.Join(", ", labels),"Grants abilities while equipped.",0);
                }
            }
        }
    }

    public class CompProperties_AbilitiesWeapon : CompProperties
    {
        // NEW: list for XML
        public List<AbilityDef> abilityDefs;

        // Legacy single field retained for backward-compat XML
        public AbilityDef abilityDef;

        public CompProperties_AbilitiesWeapon()
        {
            this.compClass = typeof(CompAbilitiesWeapon);
        }
    }
}
