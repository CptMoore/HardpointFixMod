using System;
using BattleTech;
using BattleTech.UI;
using Harmony;
using UnityEngine.EventSystems;

namespace VisualHardpointLimits
{
    [HarmonyPatch(typeof(MechLabLocationWidget), "OnMechLabDrop")]
    public static class MechLabLocationWidgetPatch
    {
        public static bool Prefix(MechLabLocationWidget __instance, PointerEventData eventData, MechLabDropTargetType addToType)
        {
            try
            {
                var vhl = new MechLabLocationWidgetPatchHelper(__instance);
                return vhl.MechLabLocationWidgetOnMechLabDrop(eventData);
            }
            catch (Exception e)
            {
                VHLLogger.Log(e);
                return true;
            }
        }
    }
}