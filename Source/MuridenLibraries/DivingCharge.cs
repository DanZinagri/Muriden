using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using VEF;
using Verse;
using Verse.AI;
using Verse.Sound;
using Ability = VEF.Abilities.Ability;

namespace MuridenLibraries;

public class DivingCharge : Ability
{
    public override void Cast(params GlobalTargetInfo[] targets)
    {
        var map = Caster.Map;
        var flyer = (MuridenJumpingPawn)PawnFlyer.MakeFlyer(
            ML_DefOf.Muriden_JumpingPawn, CasterPawn, targets[0].Cell, null, null);

        flyer.ability = this;                 // give the flyer a handle back
        flyer.landingTarget = targets[0];     // remember who/where to hit

        GenSpawn.Spawn(flyer, Caster.Position, map);

        base.Cast(targets);                   // costs/cooldown etc.

        //
        // AttackTarget(((LocalTargetInfo)targets[0]), map);
    }

    // Make this accessible from the flyer (or refactor into a static helper).
   /* private void AttackTarget(Pawn attacker, LocalTargetInfo target, Map map)
    {
        // Target pawn can move; re-resolve if needed.
        Pawn targetPawn = target.Pawn
            ?? target.Thing as Pawn
            ?? map.thingGrid.ThingAt<Pawn>(target.Cell);

        if (targetPawn == null || targetPawn.Dead || !targetPawn.Spawned) return;

        // Ensure attacker is free to swing.
        attacker.stances.SetStance(new Stance_Mobile());

        VerbProperties_MundaneMeleeDamageAmount_Patch.multiplyMundaneByPawnMeleeSkill = true;
        attacker.meleeVerbs.TryMeleeAttack(targetPawn, null, surpriseAttack: true);
        attacker.meleeVerbs.TryMeleeAttack(targetPawn, null, surpriseAttack: true);
        VerbProperties_MundaneMeleeDamageAmount_Patch.multiplyMundaneByPawnMeleeSkill = false;

        // Optional impact VFX/SFX at the landing cell
        SoundDefOf.Pawn_Melee_Punch_HitBuilding_Generic.PlayOneShot(new TargetInfo(attacker.Position, map));
    }*/
}

