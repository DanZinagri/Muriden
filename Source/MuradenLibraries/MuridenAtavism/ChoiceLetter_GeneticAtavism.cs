using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace MuridenLibraries
{
    //Class to create a letter notification when a greater foxian was borne
    public class ChoiceLetter_GeneticAtavism : ChoiceLetter
    {
        private Pawn pawn;

        public override bool CanShowInLetterStack => pawn.Faction?.IsPlayer ?? false;

        public override IEnumerable<DiaOption> Choices
        {
            get
            {
                if (base.ArchivedOnly)
                {
                    yield return base.Option_Close;
                    yield break;
                }
                else
                {
                    yield return base.Option_Close;
                }
                if (lookTargets.IsValid())
                {
                    yield return base.Option_JumpToLocationAndPostpone;
                }
            }
        }

        public void Start()
        {
            pawn = lookTargets.TryGetPrimaryTarget().Thing as Pawn;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref pawn, "pawn", saveDestroyedThings: true);
            if (Scribe.mode != LoadSaveMode.Saving && pawn == null)
            {
                pawn = lookTargets.TryGetPrimaryTarget().Thing as Pawn;
            }
        }
    }
}