using Verse;

namespace MuridenLibraries;

// Shared entry point for the charge abilities' melee strikes.
// The marker hediff is what StatPart_ChargeMeleeSkill keys off of, so the whole
// skill-scaling behaviour lives inside this bracket and nowhere else.
public static class ChargeStrike
{
    public static void DoStrikes(Pawn attacker, Thing target, int strikeCount)
    {
        if (attacker?.meleeVerbs == null || target == null) return;

        Hediff marker = attacker.health?.AddHediff(ML_DefOf.Muriden_ChargeStrike);
        try
        {
            for (int i = 0; i < strikeCount; i++)
            {
                attacker.meleeVerbs.TryMeleeAttack(target, null, surpriseAttack: true);
            }
        }
        finally
        {
            if (marker != null) attacker.health.RemoveHediff(marker);
        }
    }
}
