using HarmonyLib;
using Verse;

namespace ArrestAndRelease
{
    [HarmonyPatch(typeof(Pawn))]
    [HarmonyPatch("get_IsFreeColonist")]
    public static class Patch_Pawn_IsFreeColonist
    {
        public static void Postfix(Pawn __instance, ref bool __result)
        {
            if (__result)
            {
                if (__instance.CarriedBy != null && __instance.CarriedBy.CurJobDef == JobDefOf.ArrestAndRelease)
                {
                    __result = false;
                }
            }
        }
    }
}
