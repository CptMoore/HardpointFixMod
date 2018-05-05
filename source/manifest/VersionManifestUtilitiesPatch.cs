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
            var addendum = VersionManifestUtilities.ManifestFromCSV(VisualHardpointLimits.config.ManifestPath);
            foreach (var entry in addendum.Entries)
            {
                __result.AddOrUpdate(entry.Id, entry.FilePath, entry.Type, entry.AddedOn, entry.AssetBundleName, entry.IsAssetBundlePersistent);
            }
        }
    }
}