using System;
using BattleTech;
using Harmony;

namespace VisualHardpointLimits
{
    [HarmonyPatch(typeof(VersionManifestUtilities), "LoadDefaultManifest")]
    public static class VersionManifestUtilitiesPatch
    {
        // ReSharper disable once RedundantAssignment
        public static void Postfix(ref VersionManifest __result)
        {
            try
            {
                var addendum = VersionManifestUtilities.ManifestFromCSV(Control.ManifestPath);
                foreach (var entry in addendum.Entries)
                {
                    __result.AddOrUpdate(entry.Id, entry.FilePath, entry.Type, entry.AddedOn, entry.AssetBundleName, entry.IsAssetBundlePersistent);
                }
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogDebug(e);
            }

        }
    }
}