using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace ArrestAndRelease
{
    [HarmonyPatch(typeof(Pawn_JobTracker))]
    [HarmonyPatch("CleanupCurrentJob")]
    public static class Patch_Pawn_JobTracker_CleanupCurrentJob
    {
        public static bool Prefix(Pawn_JobTracker __instance, ref JobCondition condition)
        {
            if (__instance.curJob != null && __instance.curJob.def == JobDefOf.ArrestAndRelease && condition != JobCondition.Succeeded)
            {
                Pawn pawn = __instance.curJob.GetTarget(TargetIndex.A).Thing as Pawn;
                if (pawn != null && ((JobDriver_ArrestAndRelease)__instance.curDriver).HasCarriedTakee)
                {
                    if (!pawn.IsPrisonerOfColony)
                    {
                        pawn.guest.CapturedBy(Faction.OfPlayer, __instance.curDriver.pawn);
                    }
                }
            }
            return true;
        }
    }
}
