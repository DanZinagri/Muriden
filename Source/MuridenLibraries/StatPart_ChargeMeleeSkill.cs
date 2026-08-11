using RimWorld;
using Verse;

namespace MuridenLibraries;

// Scales melee damage by the attacker's melee skill while the charge marker hediff is present.
// Vanilla multiplies melee damage by MeleeDamageFactor inside VerbProperties.GetDamageFactorFor,
// so hooking the stat gets us the same result the old Harmony postfix did, without the patch.
public class StatPart_ChargeMeleeSkill : StatPart
{
    public HediffDef marker;
    public float divisor = 10f;

    public override void TransformValue(StatRequest req, ref float val)
    {
        if (TryGetFactor(req, out float factor))
        {
            val *= factor;
        }
    }

    public override string ExplanationPart(StatRequest req)
    {
        if (!TryGetFactor(req, out float factor)) return null;
        return marker.LabelCap + ": x" + factor.ToString("0.00");
    }

    private bool TryGetFactor(StatRequest req, out float factor)
    {
        factor = 1f;
        if (marker == null || divisor <= 0f) return false;
        if (!req.HasThing || req.Thing is not Pawn pawn) return false;
        // Animals and mechs have no skill tracker.
        if (pawn.skills == null || pawn.health?.hediffSet == null) return false;
        if (!pawn.health.hediffSet.HasHediff(marker)) return false;

        factor = pawn.skills.GetSkill(SkillDefOf.Melee).Level / divisor;
        return true;
    }
}
