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
    }
}
