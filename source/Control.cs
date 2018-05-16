
using HBS.Logging;
using Harmony;
using System.Reflection;
using DynModLib;

namespace HardpointFixMod
{
    public static class Control
    {
        public static Mod mod;

        public static HardpointSettings settings = new HardpointSettings();

        public static void Start(string modDirectory, string json)
        {
            mod = new Mod(modDirectory);
            Logger.SetLoggerLevel(mod.Logger.Name, LogLevel.Log);

            mod.LoadSettings(settings);
			
			var harmony = HarmonyInstance.Create(mod.Name);
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            // logging output can be found under BATTLETECH\BattleTech_Data\output_log.txt
            // or also under yourmod/log.txt
            mod.Logger.Log("Loaded " + mod.Name);
        }
    }

    public class HardpointSettings : ModSettings
    {
        public bool enforceHardpointLimits = true;
        public bool allowLRMsInSmallerSlotsForAll = false;
        public string[] allowLRMsInSmallerSlotsForMechs = { "atlas" };
        public bool allowLRMsInLargerSlotsForAll = true;
    }
}
