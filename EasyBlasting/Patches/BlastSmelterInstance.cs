using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EasyBlasting.Patches
{
    internal class BlastSmelterInstancePatch
    {
        [HarmonyPatch(typeof(BlastSmelterInstance), nameof(BlastSmelterInstance.ConsumeFuel))]
        [HarmonyPrefix]
        static bool ConsumeOneCharge(BlastSmelterInstance __instance, ref bool __result) {
            if (!EasyBlastingPlugin.EasyBlastSmelters.Value) return true;

            if (EasyBlastingPlugin.PauseProcessing.Value) {
                ref Inventory outputInventory = ref __instance.GetOutputInventory();
                if (!outputInventory.isEmpty && outputInventory.myStacks[0].isFull) {
                    __result = false;
                    return false;
                }
            }
            
            ref Inventory fuelInventory = ref __instance.GetFuelInventory();
            if (fuelInventory.isEmpty) return true;

            fuelInventory.RemoveResourcesFromSlot(0, 1);
            __result = true;

            return false;
        }
    }
}
