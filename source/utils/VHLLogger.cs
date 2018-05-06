using System;
using System.IO;
using BattleTech;

namespace VisualHardpointLimits
{
    internal static class VHLLogger
    {
        internal static void Log(string text)
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

        internal static void Log(Exception e)
        {
            Log(e.ToString());
        }

        internal static void Log(Exception e, string text)
        {
            Log(text);
            Log(e);
        }
    }
}