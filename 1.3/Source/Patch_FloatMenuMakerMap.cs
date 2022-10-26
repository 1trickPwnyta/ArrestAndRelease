using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace ArrestAndRelease
{
    [HarmonyPatch(typeof(FloatMenuMakerMap))]
    [HarmonyPatch("AddHumanlikeOrders")]
    public static class Patch_FloatMenuMakerMap
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            bool tryToArrest = false;
            object ldlocTargetPawnLoc = null;
            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Ldstr && instruction.operand.ToString() == "TryToArrest")
                {
                    tryToArrest = true;
                    yield return instruction;
                } 
                else if (tryToArrest && ldlocTargetPawnLoc == null)
                {
                    ldlocTargetPawnLoc = instruction.operand;
                    yield return instruction;
                }
                else if (tryToArrest && instruction.opcode == OpCodes.Callvirt && (MethodInfo)instruction.operand == Refs.m_List_FloatMenuOption_Add)
                {
                    tryToArrest = false;
                    yield return instruction;
                    yield return new CodeInstruction(OpCodes.Ldarg_2);
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    yield return new CodeInstruction(OpCodes.Ldloc_S, ldlocTargetPawnLoc);
                    yield return new CodeInstruction(OpCodes.Call, Refs.m_FloatMenuOptionTool_AddArrestAndReleaseOption);
                }
                else
                {
                    yield return instruction;
                }
            }
        }
    }
}
