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

public class BayonetCharge : Ability
{
    public override void Cast(params GlobalTargetInfo[] targets)
    {
        base.Cast(targets);
        AttackTarget(((LocalTargetInfo)targets[0]));
    }

    private void AttackTarget(LocalTargetInfo target)
    {
        IntVec3 start = pawn.Position;
        IntVec3 end = target.Cell;
        Map map = pawn.Map;

        // Optional visual: starting dust puff
        FleckMaker.Static(start, map, FleckDefOf.DustPuffThick);

        // Get a path from current position to target
        using (PawnPath path = map.pathFinder.FindPathNow(start, end, pawn))
        {
            if (path.Found)
            {
                foreach (IntVec3 cell in path.NodesReversed)
                {
                    if (!cell.Walkable(map)) continue;

                    pawn.Position = cell;
                    FleckMaker.ThrowDustPuff(cell, map, 1.2f);

                    // Optional: add short tick delay for smoother animation
                    // (If used inside a JobDriver/Toil loop, yield return, not Sleep)
                    if (cell.Walkable(map))
                    {
                        // Quick visual pacing effect — purely aesthetic, not a true delay
                        if (Rand.Chance(0.3f))
                            FleckMaker.ThrowMicroSparks(cell.ToVector3Shifted(), map);
                    }
                }
            }
        }
        this.pawn.Position = target.Cell;
        this.pawn.Notify_Teleported(false);
        pawn.stances.SetStance(new Stance_Mobile());
        VerbProperties_MundaneMeleeDamageAmount_Patch.multiplyMundaneByPawnMeleeSkill = true;
        pawn.meleeVerbs.TryMeleeAttack(target.Pawn, null, true);
        pawn.meleeVerbs.TryMeleeAttack(target.Pawn, null, true);
        VerbProperties_MundaneMeleeDamageAmount_Patch.multiplyMundaneByPawnMeleeSkill = false;

        // Optional: impact visual
        FleckMaker.Static(end, map, FleckDefOf.MicroSparks);
        SoundDefOf.Pawn_Melee_Punch_HitBuilding_Generic.PlayOneShot(new TargetInfo(end, map));
    }



    /*
    private void AttackTarget(LocalTargetInfo target)
    {
        this.AddEffecterToMaintain(EffecterDefOf.Skip_Entry.Spawn(pawn.Position, this.pawn.Map, 0.72f), pawn.Position, 60);
        //this.AddEffecterToMaintain(ML_DefOf.VPE_Skip_ExitNoDelayRed.Spawn(target.Cell, this.pawn.Map, 0.72f), target.Cell, 60);
        this.pawn.Position = target.Cell;
        this.pawn.Notify_Teleported(false);
        this.pawn.stances.SetStance(new Stance_Mobile());
        VerbProperties_MundaneMeleeDamageAmount_Patch.multiplyMundaneByPawnMeleeSkill = true;
        this.pawn.meleeVerbs.TryMeleeAttack(target.Pawn, null, true);
        this.pawn.meleeVerbs.TryMeleeAttack(target.Pawn, null, true); // deals two attack at once
        VerbProperties_MundaneMeleeDamageAmount_Patch.multiplyMundaneByPawnMeleeSkill = false;
        //castSounds.RandomElement().PlayOneShot(pawn);
    }*/
    private static List<SoundDef> castSounds = new List<SoundDef>
    {
        //ML_DefOf.VPE_Killskip_Jump_01a,
        //ML_DefOf.VPE_Killskip_Jump_01b,
        //ML_DefOf.VPE_Killskip_Jump_01c,
    };
}