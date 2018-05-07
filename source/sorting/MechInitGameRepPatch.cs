﻿using System;
using System.Linq;
using BattleTech;
using Harmony;
using UnityEngine;

namespace VisualHardpointLimits
{
    [HarmonyPatch(typeof(Mech), "InitGameRep")]
    public static class MechInitGameRepPatch
    {
        public static void Prefix(Mech __instance, Transform parentTransform)
        {
            try
            {
                var componentRefs = __instance.Weapons
                    .Select(w => w.baseComponentRef as MechComponentRef)
                    .Where(c => c != null)
                    .ToList();

                MechHardpointRulesGetComponentPrefabNamePatch.SetupCalculator(__instance.MechDef.Chassis, componentRefs);
            }
            catch (Exception e)
            {
                ModLogger.Log(e);
            }
        }

        public static void Postfix(Mech __instance, Transform parentTransform)
        {
            MechHardpointRulesGetComponentPrefabNamePatch.ResetCalculator();
        }
    }
}