using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace ArrestAndRelease
{
    public static class FloatMenuOptionTool
    {
        public static void AddArrestAndReleaseOption(List<FloatMenuOption> opts, Pawn pawn, LocalTargetInfo targetPawn)
        {
			Pawn pTarg = (Pawn)targetPawn.Thing;
			Debug.Log("target pawn: " + pTarg);
			if (ArrestAndReleaseUtil.IsUnderArrestAndRelease(pTarg))
            {
				return;
            }

			Action action = delegate ()
			{
				bool possiblePrison = pawn.Map.regionGrid.allRooms.Any((room) => room.IsSuitableForArrest(pawn, pTarg));
				if (!possiblePrison)
				{
					Messages.Message("CannotArrest".Translate() + ": " + "ArrestAndRelease_NoPotentialPrison".Translate(), pTarg, MessageTypeDefOf.RejectInput, false);
					return;
				}
				IntVec3 releaseCell;
				RCellFinder.TryFindPrisonerReleaseCell(pTarg, pawn, out releaseCell);
				Job job = JobMaker.MakeJob(JobDefOf.ArrestAndRelease, pTarg, releaseCell);
				job.count = 1;
				pawn.jobs.TryTakeOrderedJob(job, new JobTag?(JobTag.Misc), false);
			};
			opts.Add(FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("ArrestAndRelease_TryToArrestAndRelease".Translate(pTarg.LabelCap, pTarg, pTarg.GetAcceptArrestChance(pawn).ToStringPercent()), action, MenuOptionPriority.High, null, pTarg, 0f, null, null, true, 0), pawn, pTarg, "ReservedBy"));
		}
    }
}
