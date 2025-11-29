using RimWorld;
using UnityEngine;
using Verse;

namespace MuridenLibraries
{
    public class CompProperties_WeaponDraw : CompProperties
    {
        public CompProperties_WeaponDraw()
        {
            compClass = typeof(CompWeaponDraw);
        }

        public GraphicData weaponGraphicData;
        public HoldOffsetSet holdOffset;
    }

    public class CompWeaponDraw : ThingComp
    {
        public CompProperties_WeaponDraw Props => (CompProperties_WeaponDraw)props;

        private Graphic cachedGraphic;

        public Graphic WeaponGraphic
        {
            get
            {
                if (cachedGraphic == null)
                {
                    if (Props.weaponGraphicData != null)
                        cachedGraphic = Props.weaponGraphicData.GraphicColoredFor(parent);
                    else
                        cachedGraphic = parent.Graphic;
                }
                return cachedGraphic;
            }
        }

        public HoldOffset GetOffsetFor(Rot4 rot)
        {
            return Props.holdOffset?.Pick(rot)
                   ?? new HoldOffset { offset = Vector3.zero, behind = false, flip = false };
        }
    }
}
