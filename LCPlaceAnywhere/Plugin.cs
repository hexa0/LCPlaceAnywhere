using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using LCPlaceAnywhere.Patches;

namespace LCPlaceAnywhere
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    public class LCPlaceAnywhereBase : BaseUnityPlugin
    {
        private const string PLUGIN_GUID = "Hexa.LCPlaceAnywhere";
        private const string PLUGIN_NAME = "LCPlaceAnywhere";
        private const string PLUGIN_VERSION = "1.0.0.0";

        private readonly Harmony harmony = new Harmony(PLUGIN_GUID);

        private static LCPlaceAnywhereBase Instance;

        internal ManualLogSource mls;


        private void Awake()
        {
            // Plugin startup logic
            if (Instance == null)
            {
                Instance = this;
            }

            mls = BepInEx.Logging.Logger.CreateLogSource(PLUGIN_GUID);

            mls.LogInfo($"Plugin {PLUGIN_GUID} is loaded!");
            harmony.PatchAll(typeof(LCPlaceAnywhereBase));
            harmony.PatchAll(typeof(ShipBuildModeManagerPatch));
        }
    }
}