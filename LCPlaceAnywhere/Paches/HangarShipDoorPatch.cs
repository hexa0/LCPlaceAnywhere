using HarmonyLib;

namespace LCPlaceAnywhere.Patches
{
    [HarmonyPatch(typeof(HangarShipDoor))]
    internal class HangarShipDoorPatch
    {
        [HarmonyPatch("SetDoorButtonsEnabled")]
        [HarmonyPostfix]
        static void forceAllowHangarDoorToOpen(ref bool ___buttonsEnabled)
        {
            ___buttonsEnabled = true;
        }
    }
}