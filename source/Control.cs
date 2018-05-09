
using DynTechMod;
using HBS.Logging;
using Harmony;
using System.IO;
using System.Reflection;

// ReSharper disable InconsistentNaming
namespace VisualHardpointLimits
{
    public static class Control
    {
        public static Mod mod;

        public static HardpointSettings settings = new HardpointSettings();

        public static void OnInit(Mod mod)
        {
            Control.mod = mod;
            Logger.SetLoggerLevel(mod.Logger.Name, LogLevel.Log);

            mod.LoadSettings(settings);
			
			var harmony = HarmonyInstance.Create(mod.Name);
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            // logging output can be found under BATTLETECH\BattleTech_Data\output_log.txt
            // or also under yourmod/log.txt
            mod.Logger.Log("Loaded " + mod.Name);
        }

        public static string ManifestPath
        {
            get { return Path.Combine(mod.Directory, "VersionManifest.csv"); }
        }
    }

    public class HardpointSettings : ModSettings
    {
        public bool allowLRMsInSmallerSlotsForAll = false;
        public string[] allowLRMsInSmallerSlotsForMechs = { "atlas" };
        public bool allowLRMsInLargerSlotsForAll = true;
    }
}
