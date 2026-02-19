using HarmonyLib;
using Sandbox.Game.Gui;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace NaturalBlockSorting.Patches;

[HarmonyPatch]
public static class MyTerminalInventoryControllerPatch
{
    [HarmonyPatch(typeof(MyTerminalInventoryController), nameof(MyTerminalInventoryController.CompareGuiControlInventoryOwners))]
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> CompareGuiControlInventoryOwners_Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        MethodInfo originalMethod = ((Func<string, string, int>)string.Compare).Method;
        MethodInfo patchMethod = ((Func<string, string, int>)NaturalStringComparer.Compare).Method;

        foreach (var instruction in instructions)
        {
            if (instruction.opcode == OpCodes.Call && instruction.OperandIs(originalMethod))
            {
                instruction.operand = patchMethod;
            }
        }

        return instructions;
    }
}
