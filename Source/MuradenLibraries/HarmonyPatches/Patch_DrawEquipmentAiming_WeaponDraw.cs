using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI.Group;

namespace MuridenLibraries
{
    [HarmonyPatch(typeof(PawnRenderUtility))]
    [HarmonyPatch(nameof(PawnRenderUtility.DrawEquipmentAiming))]
    public static class Patch_DrawEquipmentAiming_WeaponDraw
    {
        static bool Prefix(Thing eq, Vector3 drawLoc, float aimAngle)
        {
            if (eq == null)
                return true;

            var weaponComp = eq.TryGetComp<CompWeaponDraw>();
            if (weaponComp == null)
                return true; // not our special weapon

            // Get pawn holding this thing
            var pawnEq = eq.ParentHolder as Pawn_EquipmentTracker;
            Pawn pawn = pawnEq?.pawn;
            if (pawn == null)
                return true; // no pawn – fall back to vanilla

            // Use pawn rotation for the N/S/E/W sprite
            Rot4 rot = pawn.Rotation;

            // Compute base draw position like your shield does
            Vector3 baseLoc = WeaponDrawHelper.GetAimingVector(pawn, drawLoc, rot);

            // Apply your per-rotation hold offset
            HoldOffset curOffset = weaponComp.GetOffsetFor(rot);
            Vector3 finalLoc = baseLoc + curOffset.offset;
            finalLoc.y += curOffset.behind ? -0.0390625f : 0.0390625f;

            // Use your directional graphic from the comp
            Graphic graphic = weaponComp.WeaponGraphic;
            Rot4 drawRot = curOffset.flip ? rot.Opposite : rot;

            // This is the key: Graphic.Draw respects drawSize, altitudes, etc.
            graphic.Draw(finalLoc, drawRot, eq);

            // Skip vanilla drawing
            return false;
        }
    }

    public static class WeaponDrawHelper
    {
        // Copy of the shield’s GetAimingVector logic, adapted to work in the patch
        public static Vector3 GetAimingVector(Pawn pawn, Vector3 rootLoc, Rot4 rot4)
        {
            if (pawn != null)
            {
                // copied from vanilla DrawEquipment method (same as your shield)
                Stance_Busy stance_Busy = pawn.stances.curStance as Stance_Busy;
                if (stance_Busy != null && !stance_Busy.neverAimWeapon && stance_Busy.focusTarg.IsValid)
                {
                    Vector3 a = (!stance_Busy.focusTarg.HasThing)
                        ? stance_Busy.focusTarg.Cell.ToVector3Shifted()
                        : stance_Busy.focusTarg.Thing.DrawPos;

                    float num = 0f;
                    if ((a - pawn.DrawPos).MagnitudeHorizontalSquared() > 0.001f)
                    {
                        num = (a - pawn.DrawPos).AngleFlat();
                    }

                    Vector3 drawLoc = rootLoc + new Vector3(0f, 0f, 0.4f).RotatedBy(num);
                    drawLoc.y += 9f / 245f;
                    return drawLoc;
                }
            }

            // Idle / normal carry, same offsets as your shield
            if (pawn == null || CarryWeaponOpenly(pawn))
            {
                if (rot4 == Rot4.South)
                {
                    Vector3 drawLoc2 = rootLoc + new Vector3(0f, 0f, -0.22f);
                    drawLoc2.y += 9f / 245f;
                    return drawLoc2;
                }
                else if (rot4 == Rot4.North)
                {
                    Vector3 drawLoc3 = rootLoc + new Vector3(0f, 0f, -0.11f);
                    // north kept at same altitude as your shield code
                    return drawLoc3;
                }
                else if (rot4 == Rot4.East)
                {
                    Vector3 drawLoc4 = rootLoc + new Vector3(0.2f, 0f, -0.22f);
                    drawLoc4.y += 9f / 245f;
                    return drawLoc4;
                }
                else if (rot4 == Rot4.West)
                {
                    Vector3 drawLoc5 = rootLoc + new Vector3(-0.2f, 0f, -0.22f);
                    drawLoc5.y += 9f / 245f;
                    return drawLoc5;
                }
            }

            return rootLoc;
        }

        // Same CarryWeaponOpenly logic as shield, if you want full parity:
        public static bool CarryWeaponOpenly(Pawn wearer)
        {
            if (wearer.carryTracker != null && wearer.carryTracker.CarriedThing != null)
                return false;
            if (wearer.Drafted)
                return true;
            if (wearer.CurJob != null && wearer.CurJob.def.alwaysShowWeapon)
                return true;
            if (wearer.mindState.duty != null && wearer.mindState.duty.def.alwaysShowWeapon)
                return true;

            Lord lord = wearer.GetLord();
            if (lord != null && lord.LordJob != null && lord.LordJob.AlwaysShowWeapon)
                return true;

            return false;
        }
    }
}
