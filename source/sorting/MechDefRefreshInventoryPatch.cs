using System;
using System.Linq;
using BattleTech;
using Harmony;

namespace VisualHardpointLimits
{
    [HarmonyPatch(typeof(MechDef), "RefreshInventory")]
    public static class MechDefRefreshInventoryPatch
    {
        public static void Prefix(MechDef __instance)
        {
            try
            {
                var adapter = new MechDefAdapter(__instance);
                if (adapter.Chassis?.HardpointDataDef == null)
                {
                    return;
                }

                var componentRefs = adapter.Inventory
                    .Where(c => c != null).Do(c =>
                    {
                        if (c.DataManager == null)
                        {
                            c.DataManager = adapter.DataManager;
                        }

                        c.RefreshComponentDef();
                    })
                    .Where(c => !c.hasPrefabName)
                    .ToList();
                MechHardpointRulesGetComponentPrefabNamePatch.SetupCalculator(adapter.Chassis, componentRefs);
            }
            catch (Exception e)
            {
                ModLogger.Log(e);
            }
        }

        public static void Postfix(MechDef __instance)
        {
            MechHardpointRulesGetComponentPrefabNamePatch.ResetCalculator();
        }
    }
}