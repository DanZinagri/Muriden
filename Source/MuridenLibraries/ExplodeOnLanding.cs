using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace MuridenLibraries;


public class CompProperties_ExplodeOnLanding : CompProperties_AbilityEffect
{
    public float radius = 10f;
    public DamageDef damageDef = DamageDefOf.Bomb;
    public int damageAmount = -1; // use -1 for default by damageDef if you want
    public SoundDef MLsound;
    public bool casterImmune = true;
    public bool damageFalloff = false;
    public bool radiusOnPawnSize = false;
    public bool damageOnPawnSize = false;

    // Visual shockwave
    public FleckDef shockwaveFleck = FleckDefOf.PsycastAreaEffect;
    public float shockwaveScale = 2f;
    public bool shockwaveScaleWithRadius = true;

    public CompProperties_ExplodeOnLanding()
    {
        compClass = typeof(CompAbilityEffect_ExplodeOnLanding);
    }
}

public class CompAbilityEffect_ExplodeOnLanding : CompAbilityEffect
{
    public CompProperties_ExplodeOnLanding MLProps => (CompProperties_ExplodeOnLanding)props;

    public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
    {
        var pawn = parent.pawn;
        if (pawn == null || pawn.Map == null) return;

        // Predict landing: prefer dest, fall back to target cell
        IntVec3 landing = dest.IsValid ? dest.Cell : (target.IsValid ? target.Cell : pawn.Position);

        // Make sure it’s standable-ish so you don’t explode into a wall
        if (!landing.InBounds(pawn.Map) || !landing.Walkable(pawn.Map))
            CellFinder.TryFindRandomCellNear(landing, pawn.Map, 2, c => c.Walkable(pawn.Map), out landing);

        float radius = MLProps.radius * (MLProps.radiusOnPawnSize ? pawn.BodySize : 1f);
        int damage = MLProps.damageAmount * (MLProps.damageOnPawnSize ? (int)(pawn.BodySize) : 1);
        Thing instigator = MLProps.casterImmune ? null : pawn;

        GenExplosion.DoExplosion(
            center: landing,
            map: pawn.Map,
            radius: radius,
            damType: MLProps.damageDef,
            instigator: instigator,
            damAmount: damage,
            armorPenetration: -1f,
            explosionSound: MLProps.sound,
            weapon: null,
            projectile: null,
            intendedTarget: null,
            postExplosionSpawnThingDef: null,
            postExplosionSpawnChance: 0f,
            postExplosionSpawnThingCount: 1,
            applyDamageToExplosionCellsNeighbors: MLProps.damageFalloff,
            chanceToStartFire: 0f,
            damageFalloff: MLProps.damageFalloff
        );

        if (MLProps.shockwaveFleck != null)
        {
            float visualScale = MLProps.shockwaveScaleWithRadius
                ? radius * MLProps.shockwaveScale
                : MLProps.shockwaveScale;

            FleckMaker.Static(
                landing.ToVector3Shifted(),
                pawn.Map,
                MLProps.shockwaveFleck,
                visualScale
            );
        }
    }

}


