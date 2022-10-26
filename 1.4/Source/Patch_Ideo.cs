using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace ArrestAndRelease
{
    [HarmonyPatch(typeof(Ideo))]
    [HarmonyPatch("RecacheColonistBelieverCount")]
    public static class Patch_Ideo_RecacheColonistBelieverCount
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            bool insert = false;
            CodeInstruction previousInstruction = null;
            CodeInstruction loadPawn = null;
            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Callvirt && (MethodInfo) instruction.operand == Refs.m_Pawn_get_IsSlave)
                {
                    insert = true;
                    loadPawn = previousInstruction;
                    yield return instruction;
                }
                else if (insert && instruction.opcode == OpCodes.Brtrue_S)
                {
                    insert = false;
                    yield return instruction;
                    yield return loadPawn;
                    yield return new CodeInstruction(OpCodes.Call, Refs.m_ArrestAndReleaseUtil_IsUnderArrestAndRelease);
                    yield return instruction;
                } 
                else
                {
                    yield return instruction;
                }

                previousInstruction = instruction;
            }
        }
    }
}
