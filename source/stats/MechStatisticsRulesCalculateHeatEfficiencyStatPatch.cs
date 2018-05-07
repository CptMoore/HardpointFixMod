using System;
using System.Linq;
using BattleTech;
using Harmony;
using UnityEngine;

namespace VisualHardpointLimits
{
    [HarmonyPatch(typeof(MechStatisticsRules), "CalculateHeatEfficiencyStat")]
    public static class MechStatisticsRulesCalculateHeatEfficiencyStatPatch
    {
        private static CombatGameConstants Combat;

        public static bool Prefix(MechDef mechDef, ref float currentValue, ref float maxValue)
        {
            try
            {
                if (Combat == null)
                {
                    Combat = CombatGameConstants.CreateFromSaved(UnityGameInstance.BattleTechGame);
                }
                var totalHeatSinkDissipation = Combat.Heat.InternalHeatSinkCount * Combat.Heat.DefaultHeatSinkDissipationCapacity;
                var heatGenerationWeapons = 0f;
                var numberOfJumpJets = 0;

                foreach (var mechComponentRef in mechDef.Inventory)
                {
                    if (mechComponentRef.Def == null)
                    {
                        mechComponentRef.RefreshComponentDef();
                    }
                    if (mechComponentRef.Def is WeaponDef weaponDef)
                    {
                        heatGenerationWeapons += weaponDef.HeatGenerated;
                    }
                    else if (mechComponentRef.ComponentDefType == ComponentType.JumpJet)
                    {
                        if (mechComponentRef.DamageLevel < ComponentDamageLevel.NonFunctional)
                        {
                            numberOfJumpJets++;
                        }
                    }
                    else if (mechComponentRef.Def is HeatSinkDef heatSinkDef)
                    {
                        totalHeatSinkDissipation += heatSinkDef.DissipationCapacity;
                    }
                }

                ModLogger.Log("heatGenerationWeapons=" + heatGenerationWeapons);
                ModLogger.Log("totalHeatSinkDissipation=" + totalHeatSinkDissipation);

                var maxHeat = Combat.Heat.MaxHeat;
                {
                    var stats = new StatCollection();
                    var maxHeatStatistic = stats.AddStatistic("MaxHeat", maxHeat);
                    var heatGeneratedStatistic = stats.AddStatistic("HeatGenerated", heatGenerationWeapons);

                    foreach (var mechComponentRef in mechDef.Inventory)
                    {
                        if (mechComponentRef.Def?.statusEffects == null)
                        {
                            continue;
                        }

                        var statusEffects = mechComponentRef.Def.statusEffects;
                        foreach (var effect in statusEffects)
                        {
                            switch (effect.statisticData.statName)
                            {
                                case "MaxHeat":
                                    stats.PerformOperation(maxHeatStatistic, effect.statisticData);
                                    break;
                                case "HeatGenerated" when effect.statisticData.targetCollection == StatisticEffectData.TargetCollection.Weapon:
                                    stats.PerformOperation(heatGeneratedStatistic, effect.statisticData);
                                    break;
                            }
                        }
                    }
                    
                    maxHeat = maxHeatStatistic.CurrentValue.Value<int>();
                    heatGenerationWeapons = heatGeneratedStatistic.CurrentValue.Value<float>();
                }

                ModLogger.Log("maxHeat=" + maxHeat);
                ModLogger.Log("heatGenerationWeapons=" + heatGenerationWeapons);

                if (numberOfJumpJets >= Combat.MoveConstants.MoveTable.Length)
                {
                    numberOfJumpJets = Combat.MoveConstants.MoveTable.Length - 1;
                }

                var heatGenerationJumpJets = 0f;
                var jumpHeatDivisor = 3;
                if (numberOfJumpJets > 0)
                {
                    heatGenerationJumpJets += numberOfJumpJets * Combat.Heat.JumpHeatUnitSize / jumpHeatDivisor;
                }
                else
                {
                    heatGenerationJumpJets = 0f;
                }

                totalHeatSinkDissipation *= Combat.Heat.GlobalHeatSinkMultiplier;
                var totalHeatGeneration = (heatGenerationWeapons + heatGenerationJumpJets) * Combat.Heat.GlobalHeatIncreaseMultiplier;

                ModLogger.Log("totalHeatGeneration=" + totalHeatGeneration);

                // rounding steps for heatSinkDissipation
                var heatDissipationPercent = Mathf.Min(totalHeatSinkDissipation / totalHeatGeneration * 100f, UnityGameInstance.BattleTechGame.MechStatisticsConstants.MaxHeatEfficiency);
                heatDissipationPercent = Mathf.Max(heatDissipationPercent, UnityGameInstance.BattleTechGame.MechStatisticsConstants.MinHeatEfficiency);

                ModLogger.Log("heatDissipationPercent=" + heatDissipationPercent);

                totalHeatSinkDissipation = totalHeatGeneration * (heatDissipationPercent / 100f);

                ModLogger.Log("totalHeatSinkDissipation=" + totalHeatSinkDissipation);

                var heatLeftOver = totalHeatGeneration - totalHeatSinkDissipation;
                var unusedHeatCapacity = maxHeat - heatLeftOver;

                ModLogger.Log("heatLeftOver=" + heatLeftOver);
                ModLogger.Log("unusedHeatCapacity=" + unusedHeatCapacity);

                currentValue = Mathf.Round((unusedHeatCapacity / maxHeat) * 10f);
                currentValue = Mathf.Max(Mathf.Min(currentValue, 10f), 1f);
                maxValue = 10f;
                return false;
            }
            catch (Exception e)
            {
                ModLogger.Log(e);
                return true;
            }
        }


        private static void PerformOperation(this StatCollection collection, Statistic statistic, StatisticEffectData data)
        {
            var type = Type.GetType(data.modType);
            var variant = new Variant(type);
            variant.SetValue(data.modValue);
            variant.statName = data.statName;
            collection.PerformOperation(statistic, data.operation, variant);
        }
    }
}