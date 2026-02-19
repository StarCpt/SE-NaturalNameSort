using HarmonyLib;
using Sandbox.Game.Entities.Cube;
using Sandbox.Game.GameSystems;
using Sandbox.Game.Screens.Helpers;

namespace NaturalBlockSorting.Patches;

[HarmonyPatch]
public static class MyTerminalComparerPatch
{
    [HarmonyPatch(typeof(MyTerminalComparer), nameof(MyTerminalComparer.Compare), [ typeof(MyTerminalBlock), typeof(MyTerminalBlock) ])]
    [HarmonyPrefix]
    public static bool Compare_MyTerminalBlock_Prefix(ref int __result, MyTerminalBlock lhs, MyTerminalBlock rhs)
    {
        string leftName = lhs.CustomName?.ToString() ?? lhs.DefinitionDisplayNameText;
        string rightName = rhs.CustomName?.ToString() ?? rhs.DefinitionDisplayNameText;

        int comparison = NaturalStringComparer.Compare(leftName, rightName);
        __result = comparison != 0 ? comparison : lhs.NumberInGrid.CompareTo(rhs.NumberInGrid);
        return false;
    }

    [HarmonyPatch(typeof(MyTerminalComparer), nameof(MyTerminalComparer.Compare), [ typeof(MyBlockGroup), typeof(MyBlockGroup) ])]
    [HarmonyPrefix]
    public static bool Compare_MyBlockGroup_Prefix(ref int __result, MyBlockGroup x, MyBlockGroup y)
    {
        __result = NaturalStringComparer.Compare(x.Name.ToString(), y.Name.ToString());
        return false;
    }
}
