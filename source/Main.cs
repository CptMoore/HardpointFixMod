using System.IO;
using Harmony;
using BattleTech;
using System.Reflection;
using BattleTech.Data;
using HBS.Util;
using TMPro;

// ReSharper disable InconsistentNaming
namespace VisualHardpointLimits
{
    public static class VisualHardpointLimits
    {
        private static string ModName = "VisualHardpointLimits";

        internal static Configuration config;

        public static void Init()
        {
            config = new Configuration();
            config.Load();

            var harmony = HarmonyInstance.Create(ModName);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public static void Reset()
        {
        }

        public class Configuration
        {
            public bool allowLRMsInSmallerSlotsForAll = false;
            public string[] allowLRMsInSmallerSlotsForMechs = { "atlas" };
            public bool allowLRMsInLargerSlotsForAll = true;
            public bool enableLogging = true;

            public string ModDirectory => Path.Combine(Path.GetDirectoryName(VersionManifestUtilities.MANIFEST_FILEPATH), @"..\..\..\Mods\VisualHardpointLimits\");
            public string ConfigPath => Path.Combine(ModDirectory, "Configuration.json");
            public string ManifestPath => Path.Combine(ModDirectory, "VersionManifest.csv");
            public string LogPath => Path.Combine(ModDirectory, @"log.txt");

            public void Load()
            {
                if (!File.Exists(ConfigPath))
                {
                    return;
                }

                string json;
                using (var reader = new StreamReader(ConfigPath))
                {
                    json = reader.ReadToEnd();
                }
                JSONSerializationUtility.FromJSON(this, json);
            }

            public void Save()
            {
                var json = JSONSerializationUtility.ToJSON(this);
                using (var writer = new StreamWriter(ConfigPath))
                {
                    writer.Write(json);
                }
            }
        }
    }
}
