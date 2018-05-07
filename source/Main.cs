using System;
using System.IO;
using Harmony;
using System.Reflection;
using HBS.Logging;
using HBS.Util;
using System.Linq;
using Microsoft.CSharp;
using System.CodeDom.Compiler;

// ReSharper disable InconsistentNaming
namespace VisualHardpointLimits
{
    public static class VisualHardpointLimits
    {
        public const string ModName = "VisualHardpointLimits";

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
            public bool enableLogging = false;


            public string AssemblyPath => Assembly.GetExecutingAssembly().Location;
            public string ModDirectory => Path.Combine(Path.GetDirectoryName(AssemblyPath), ModName);
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

    public static class ModLogger
    {
        public static void Log(string text)
        {
            if (!VisualHardpointLimits.config.enableLogging)
            {
                return;
            }

            using (var writer = new StreamWriter(VisualHardpointLimits.config.LogPath, true))
            {
                writer.WriteLine(new DateTime() + " " + text);
            }
        }

        public static void Log(Exception e)
        {
            Log(e.ToString());
        }

        public static void Log(Exception e, string text)
        {
            Log(text);
            Log(e);
        }
    }
}
