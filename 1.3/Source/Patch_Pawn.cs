using HarmonyLib;
using Verse;

namespace ArrestAndRelease
{
    [HarmonyPatch(typeof(Pawn))]
    [HarmonyPatch("get_IsPrisoner")]
    public static class Patch_Pawn_get_IsPrisoner
    {
        public static void Postfix(Pawn __instance, ref bool __result) 
        {
            if (!__result)
            {
                __result = ArrestAndReleaseUtil.IsUnderArrestAndRelease(__instance);
            }
        }
    }

    [HarmonyPatch(typeof(Pawn))]
    [HarmonyPatch("get_IsFreeNonSlaveColonist")]
    public static class Patch_Pawn_get_IsFreeNonSlaveColonist
    {
        public static void Postfix(Pawn __instance, ref bool __result)
        {
            if (__result)
            {
                __result = !ArrestAndReleaseUtil.IsUnderArrestAndRelease(__instance);
            }
        }
    }
}
