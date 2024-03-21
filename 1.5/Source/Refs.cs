using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using Verse;

namespace ArrestAndRelease
{
    public static class Refs
    {
        public static MethodInfo m_List_FloatMenuOption_Add = AccessTools.Method(typeof(List<FloatMenuOption>), "Add");
        public static MethodInfo m_FloatMenuOptionTool_AddArrestAndReleaseOption = AccessTools.Method(typeof(FloatMenuOptionTool), "AddArrestAndReleaseOption");
        public static MethodInfo m_Pawn_get_IsSlave = AccessTools.Method(typeof(Pawn), "get_IsSlave");
        public static MethodInfo m_ArrestAndReleaseUtil_IsUnderArrestAndRelease = AccessTools.Method(typeof(ArrestAndReleaseUtil), "IsUnderArrestAndRelease");
        public static MethodInfo m_Pawn_get_IsPrisoner = AccessTools.Method(typeof(Pawn), "get_IsPrisoner");
    }
}
