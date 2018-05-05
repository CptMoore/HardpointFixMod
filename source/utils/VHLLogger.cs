using System;
using System.IO;

namespace VisualHardpointLimits
{
    internal static class VHLLogger
    {
        internal static void Log(string text)
        {
            //using (var writer = new StreamWriter("D:\\log.txt", true))
            //{
            //    writer.WriteLine(new DateTime() + " " + text);
            //}
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