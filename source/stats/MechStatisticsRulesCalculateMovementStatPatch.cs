using System;
using System.Linq;
using BattleTech;
using Harmony;
using UnityEngine;

namespace VisualHardpointLimits
{
    [HarmonyPatch(typeof(MechStatisticsRules), "CalculateMovementStat")]
    public static class MechStatisticsRulesCalculateMovementStatPatch
    {
        public static bool Prefix(MechDef mechDef, ref float currentValue, ref float maxValue)
        {
            try
            {
                var maxSprintDistance = mechDef.Chassis.MovementCapDef.MaxSprintDistance;
                currentValue = Mathf.Floor(
                    (maxSprintDistance - UnityGameInstance.BattleTechGame.MechStatisticsConstants.MinSprintFactor)
                    / (UnityGameInstance.BattleTechGame.MechStatisticsConstants.MaxSprintFactor - UnityGameInstance.BattleTechGame.MechStatisticsConstants.MinSprintFactor)
                    * 10f
                );
                currentValue = Mathf.Max(currentValue, 1f);
                maxValue = 10f;
                return false;
            }
            catch (Exception e)
            {
                ModLogger.Log(e);
                return true;
            }
        }
    }
}