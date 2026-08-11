using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using Verse;
using Verse.AI;
using Verse.Sound;
using Ability = VEF.Abilities.Ability;

namespace MuridenLibraries;

public class BayonetCharge : Ability
{
    public override void Cast(params GlobalTargetInfo[] targets)
    {
        base.Cast(targets);
        AttackTarget((LocalTargetInfo)targets[0]);
    }

    private void AttackTarget(LocalTargetInfo target)
    {
        Map map = pawn.Map;
        if (map == null) return;

        Pawn targetPawn = target.Pawn ?? target.Thing as Pawn;
        if (targetPawn == null || targetPawn.Dead || !targetPawn.Spawned) return;

        IntVec3 start = pawn.Position;
        IntVec3 end = target.Cell;

        // Dust puff at both ends of the charge. The pawn covers the ground in one
        // frame, so there is no in-between animation to draw here.
        FleckMaker.Static(start, map, FleckDefOf.DustPuffThick);

        pawn.Position = end;
        pawn.Notify_Teleported(false);
        pawn.stances.SetStance(new Stance_Mobile());

        ChargeStrike.DoStrikes(pawn, targetPawn, 2);

        FleckMaker.Static(end, map, FleckDefOf.MicroSparks);
        SoundDefOf.Pawn_Melee_Punch_HitBuilding_Generic.PlayOneShot(new TargetInfo(end, map));
    }

    private static List<SoundDef> castSounds = new List<SoundDef>
    {
        //ML_DefOf.VPE_Killskip_Jump_01a,
        //ML_DefOf.VPE_Killskip_Jump_01b,
        //ML_DefOf.VPE_Killskip_Jump_01c,
    };
}
