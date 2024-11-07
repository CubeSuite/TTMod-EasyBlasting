using EquinoxsModUtils;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace EasyBlasting.Patches
{
    internal class DrillInstancePatch
    {
        internal static int blast2 = -1;
        internal static int blast3 = -1;
        internal static int blast4 = -1;
        internal static int blast5 = -1;

        [HarmonyPatch(typeof(DrillInstance), nameof(DrillInstance.ConsumeExplosives))]
        [HarmonyPrefix]
        static bool ConsumeMinExplosives(DrillInstance __instance, out int explosivesConsumed, ref bool __result) {
            explosivesConsumed = 0;
            if (!EasyBlastingPlugin.EasyBlastMiners.Value) return true;

            if (blast2 == -1) GetBlastUnlockIds();

            List<int> validConsumptionAmounts = new List<int>() { 1 };
            if (TechTreeState.instance.IsUnlockActive(blast2)) validConsumptionAmounts.Add(3);
            if (TechTreeState.instance.IsUnlockActive(blast3)) validConsumptionAmounts.Add(5);
            if (TechTreeState.instance.IsUnlockActive(blast4)) validConsumptionAmounts.Add(10);
            if (TechTreeState.instance.IsUnlockActive(blast5)) validConsumptionAmounts.Add(15);

            ref Inventory fuelInventory = ref __instance.GetFuelInventory();
            int numCharges = fuelInventory.myStacks[0].count;
            int toConsume = validConsumptionAmounts.Where(amount => amount <= numCharges).Max();

            explosivesConsumed = toConsume;
            fuelInventory.RemoveResourcesFromSlot(0, toConsume);
            __result = true;

            return false;
        }

        [HarmonyPatch(typeof(DrillInstance), "ExplosivesReady")]
        [HarmonyPrefix]
        static bool ReplaceExplosivesReady(ref DrillInstance __instance, ref bool __result) {
            if (__instance.GetOutputInventory().myStacks[0].isEmpty) return true;

            ResourceInfo minedResource = __instance.GetOutputInventory().myStacks[0].info;
            int maxStack = __instance.GetOutputInventory().myStacks[0].maxStack;
            int have = __instance.GetOutputInventory().GetResourceCount(minedResource.uniqueId);

            if (have == maxStack) {
                __result = false;
                return false;
            }

            return true;
        }

        // Private Functions

        private static void GetBlastUnlockIds() {
            blast2 = EMU.Unlocks.GetUnlockByName(EMU.Names.Unlocks.BDRMultiBlastII).uniqueId;
            blast3 = EMU.Unlocks.GetUnlockByName(EMU.Names.Unlocks.BDRMultiBlastIII).uniqueId;
            blast4 = EMU.Unlocks.GetUnlockByName(EMU.Names.Unlocks.BDRMultiBlastIV).uniqueId;
            blast5 = EMU.Unlocks.GetUnlockByName(EMU.Names.Unlocks.BDRMultiBlastV).uniqueId;
        }
    }
}
