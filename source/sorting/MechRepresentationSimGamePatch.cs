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
                MechHardpointRulesGetComponentPrefabNamePatch.SetupCalculator(
                    __instance.mechDef != null ? __instance.mechDef.Chassis : null,
                    __instance.mechDef != null ? (__instance.mechDef.Inventory != null ? __instance.mechDef.Inventory.ToList() : null) : null);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogDebug(e);
            }
        }

        public static void Postfix(MechRepresentationSimGame __instance)
        {
            MechHardpointRulesGetComponentPrefabNamePatch.ResetCalculator();
        }
    }
}