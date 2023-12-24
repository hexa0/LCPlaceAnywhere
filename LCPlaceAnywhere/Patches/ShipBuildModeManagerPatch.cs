using HarmonyLib;

namespace LCPlaceAnywhere.Patches
{
    [HarmonyPatch(typeof(ShipBuildModeManager))]
    internal class ShipBuildModeManagerPatch
    {
        [HarmonyPatch("EnterBuildMode")]
        [HarmonyPrefix]
        static void forceAllowBuildPatch(ref float ___timeSincePlacingObject, ref bool ___CanConfirmPosition)
        {
            ___timeSincePlacingObject = 1.69420f;
            ___CanConfirmPosition = true;
        }

        [HarmonyPatch("PlayerMeetsConditionsToBuild")]
        [HarmonyPostfix]
        static void forceAllowBuildOutsideShip(ref bool __result)
        {
            __result = true;
        }
    }
}