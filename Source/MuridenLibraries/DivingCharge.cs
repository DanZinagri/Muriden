using RimWorld;
using RimWorld.Planet;
using Verse;
using Ability = VEF.Abilities.Ability;

namespace MuridenLibraries;

public class DivingCharge : Ability
{
    public override void Cast(params GlobalTargetInfo[] targets)
    {
        var map = Caster.Map;
        var flyer = (MuridenJumpingPawn)PawnFlyer.MakeFlyer(
            ML_DefOf.Muriden_JumpingPawn, CasterPawn, targets[0].Cell, null, null);

        // AbilityPawnFlyer.RespawnPawn needs this for ApplyHediffs and the landing stance,
        // and MuridenJumpingPawn reads it back through DiveAbility for the landing strike.
        flyer.ability = this;                 // give the flyer a handle back
        flyer.landingTarget = targets[0];     // remember who/where to hit

        GenSpawn.Spawn(flyer, Caster.Position, map);

        base.Cast(targets);                   // costs/cooldown etc.
    }
}
