using System;
using System.Linq;
using BattleTech;
using Harmony;

namespace VisualHardpointLimits
{
    [HarmonyPatch(typeof(MechRepresentationSimGame), "LoadWeapons")]
    public static class MechRepresentationSimGameLoadWeaponsPatch
    {
        public static void Prefix(MechRepresentationSimGame __instance)
        {
            try
            {
                MechHardpointRulesGetComponentPrefabNamePatch.SetupCalculator(__instance.mechDef?.Chassis, __instance.mechDef?.Inventory?.ToList());
            }
            catch (Exception e)
            {
                ModLogger.Log(e);
            }
        }

        public static void Postfix(MechRepresentationSimGame __instance)
        {
            MechHardpointRulesGetComponentPrefabNamePatch.ResetCalculator();
        }
    }
}