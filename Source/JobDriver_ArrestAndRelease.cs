using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace ArrestAndRelease
{
    public class JobDriver_ArrestAndRelease : JobDriver
    {
        public Pawn Takee
        {
            get
            {
                return (Pawn)this.job.GetTarget(TargetIndex.A).Thing;
            }
        }

		public bool HasCarriedTakee
        {
			get
            {
				return hasCarriedTakeeInt;
            }
        }

		private bool hasCarriedTakeeInt = false;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return this.pawn.Reserve(this.Takee, this.job, 1, -1, null, errorOnFailed);
        }

		protected override IEnumerable<Toil> MakeNewToils()
		{
			hasCarriedTakeeInt = false;

			this.FailOnDestroyedOrNull(TargetIndex.A);
			this.FailOnDowned(TargetIndex.A);
			this.FailOnAggroMentalStateAndHostile(TargetIndex.A);
			this.FailOn(() => this.Takee.IsPrisonerOfColony);
			Toil goToTakee = Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch)
					.FailOnDespawnedNullOrForbidden(TargetIndex.A)
					.FailOn(() => !this.Takee.CanBeArrestedBy(this.pawn))
					.FailOnSomeonePhysicallyInteracting(TargetIndex.A)
					.FailOn(() => !this.pawn.Map.regionGrid.allRooms.Any((room) => room.IsSuitableForArrest(this.pawn, this.Takee)));
			Toil checkArrestResistance = new Toil();
			checkArrestResistance.initAction = delegate ()
			{
				Pawn pawn = (Pawn)this.job.targetA.Thing;
				Lord lord = pawn.GetLord();
				if (lord != null)
				{
					lord.Notify_PawnAttemptArrested(pawn);
				}
				GenClamor.DoClamor(pawn, 10f, ClamorDefOf.Harm);
				if (!pawn.IsPrisoner && !pawn.IsSlave)
				{
					QuestUtility.SendQuestTargetSignals(pawn.questTags, "Arrested", pawn.Named("SUBJECT"));
					if (pawn.Faction != null)
					{
						QuestUtility.SendQuestTargetSignals(pawn.Faction.questTags, "FactionMemberArrested", pawn.Faction.Named("FACTION"));
					}
				}
				if (!pawn.CheckAcceptArrest(this.pawn))
				{
					this.pawn.jobs.EndCurrentJob(JobCondition.Incompletable, true, true);
				}
			};
			yield return Toils_Jump.JumpIf(checkArrestResistance, () => this.pawn.IsCarryingPawn(this.Takee));
			yield return goToTakee;
			yield return checkArrestResistance;
			Toil startCarrying = Toils_Haul.StartCarryThing(TargetIndex.A, false, false, false);
			startCarrying.AddFinishAction(delegate
			{
				hasCarriedTakeeInt = true;

				if (this.Takee.IsCarryingPawn())
				{
					Thing thing;
					this.Takee.carryTracker.TryDropCarriedThing(this.Takee.PositionHeld, ThingPlaceMode.Near, out thing);
				}

				this.psychologicallyImprisonTakee();
			});
			yield return startCarrying;
			yield return Toils_General.Do(delegate
			{
				if (!this.job.ritualTag.NullOrEmpty())
				{
					Lord lord = this.Takee.GetLord();
					LordJob_Ritual lordJob_Ritual = ((lord != null) ? lord.LordJob : null) as LordJob_Ritual;
					if (lordJob_Ritual != null)
					{
						lordJob_Ritual.AddTagForPawn(this.Takee, this.job.ritualTag);
					}
					Lord lord2 = this.pawn.GetLord();
					lordJob_Ritual = (((lord2 != null) ? lord2.LordJob : null) as LordJob_Ritual);
					if (lordJob_Ritual != null)
					{
						lordJob_Ritual.AddTagForPawn(this.pawn, this.job.ritualTag);
					}
				}
			});
			Toil carryToCell = Toils_Haul.CarryHauledThingToCell(TargetIndex.B, PathEndMode.ClosestTouch);
			carryToCell.FailOn(() => !this.pawn.IsCarryingPawn(this.Takee));
			yield return carryToCell;
			yield return Toils_Haul.PlaceHauledThingInCell(TargetIndex.B, carryToCell, false, false);
			Toil setReleased = new Toil();
			setReleased.initAction = delegate ()
			{
				Pawn pawn = setReleased.actor.jobs.curJob.targetA.Thing as Pawn;
				pawn.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOf.WasImprisoned, null, null);
				pawn.guest.ClearLastRecruiterData();
				if (!PawnBanishUtility.WouldBeLeftToDie(pawn, pawn.Map.Tile))
				{
					GenGuest.AddHealthyPrisonerReleasedThoughts(pawn);
				}
				QuestUtility.SendQuestTargetSignals(pawn.questTags, "Released", pawn.Named("SUBJECT"));
			};
			yield return setReleased;
			yield break;
		}

		private void psychologicallyImprisonTakee()
		{
			if (!this.Takee.IsPrisonerOfColony)
			{
				this.Takee.ClearMind(true, false, true);
				Lord lord = this.Takee.GetLord();
				if (lord != null)
				{
					lord.Notify_PawnLost(this.Takee, PawnLostCondition.MadePrisoner, null);
				}
				TaleRecorder.RecordTale(TaleDefOf.Captured, new object[]
				{
					this.pawn,
					this.Takee
				});
				this.pawn.records.Increment(RecordDefOf.PeopleCaptured);
				if (this.Takee.Ideo != null)
                {
					this.Takee.Ideo.RecacheColonistBelieverCount();
                }
			}
		}
	}
}
