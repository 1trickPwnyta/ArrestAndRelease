using RimWorld;
using System.Linq;
using Verse;
using Verse.AI;

namespace ArrestAndRelease
{
    public class FloatMenuOptionProvider_ArrestAndRelease : FloatMenuOptionProvider_Arrest
    {
        protected override FloatMenuOption GetSingleOptionFor(Pawn clickedPawn, FloatMenuContext context)
        {
            Pawn arrestor = context.FirstSelectedPawn;

            if (!clickedPawn.CanBeArrestedBy(arrestor))
            {
                return null;
            }

            if (clickedPawn.IsPrisoner)
            {
                return null;
            }

            if (clickedPawn.Downed && clickedPawn.guilt.IsGuilty)
            {
                return null;
            }

            if (ArrestAndReleaseUtil.IsUnderArrestAndRelease(clickedPawn))
            {
                return null;
            }

            if (arrestor.InSameExtraFaction(clickedPawn, ExtraFactionType.HomeFaction) || arrestor.InSameExtraFaction(clickedPawn, ExtraFactionType.MiniFaction))
            {
                return new FloatMenuOption("ArrestAndRelease_CannotArrestAndRelease".Translate() + ": " + "SameFaction".Translate(clickedPawn), null);
            }

            if (!arrestor.CanReach(clickedPawn, PathEndMode.OnCell, Danger.Deadly))
            {
                return new FloatMenuOption("ArrestAndRelease_CannotArrestAndRelease".Translate() + ": " + "NoPath".Translate().CapitalizeFirst(), null);
            }

            return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("ArrestAndRelease_TryToArrestAndRelease".Translate(clickedPawn.LabelShortCap, clickedPawn.GetAcceptArrestChance(arrestor).ToStringPercent()), () =>
            {
                bool possiblePrison = clickedPawn.Map.regionGrid.AllRooms.Any(r => r.IsSuitableForArrest(arrestor, clickedPawn));
                if (possiblePrison)
                {
                    RCellFinder.TryFindPrisonerReleaseCell(clickedPawn, arrestor, out IntVec3 releaseCell);
                    Job job = JobMaker.MakeJob(JobDefOf.ArrestAndRelease, clickedPawn, releaseCell);
                    job.count = 1;
                    arrestor.jobs.TryTakeOrderedJob(job);
                }
                else
                {
                    Messages.Message("CannotArrest".Translate() + ": " + "ArrestAndRelease_NoPotentialPrison".Translate(), clickedPawn, MessageTypeDefOf.RejectInput, false);
                }
            }), arrestor, clickedPawn);
        }
    }
}
