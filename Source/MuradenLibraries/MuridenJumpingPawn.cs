using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using VEF.Abilities;
using Verse;
using Verse.Sound;

namespace MuridenLibraries;

//i don't want to make VPE a requirement so lifted some of the logic.
public class MuridenJumpingPawn : AbilityPawnFlyer
{
    public DivingCharge ability;              // set by DivingCharge.Cast
    public GlobalTargetInfo landingTarget;    // set by DivingCharge.Cast

    public override void DynamicDrawPhaseAt(DrawPhase phase, Vector3 drawLoc, bool flip = false)
    {
        FlyingPawn.Drawer.renderer.DynamicDrawPhaseAt(phase, drawLoc, Rotation, true);
    }

    protected override void RespawnPawn()
    {
        var fp = FlyingPawn;          // cached before base to use for flecks etc.
        base.RespawnPawn();           // pawn is now on the map at the landing cell

        // VFX on touchdown
        FleckMaker.ThrowSmoke(fp.DrawPos, fp.Map, 1f);
        FleckMaker.ThrowDustPuffThick(fp.DrawPos, fp.Map, 2f, new(1f, 1f, 1f, 2.5f));

        // Now that we’ve landed, do the actual attack.
        if (ability != null && fp?.Spawned == true)
        {
            var map = fp.Map;

            // Prefer a live pawn at/near the target cell. If the original moved, try who’s in the cell now.
            LocalTargetInfo lti = landingTarget.IsValid ? new LocalTargetInfo(landingTarget.Cell) : new LocalTargetInfo(fp.Position);
            Pawn maybe = landingTarget.Pawn ?? map.thingGrid.ThingAt<Pawn>(lti.Cell);
            if (maybe != null) lti = maybe;

            AttackTarget(fp, lti, map);
        }
    }

    private void AttackTarget(Pawn attacker, LocalTargetInfo target, Map map)
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
    }

    // (rest of your class unchanged)
}

