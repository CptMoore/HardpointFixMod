
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Harmony;
using BattleTech;
using System.Reflection;
using BattleTech.UI;
using HBS.Util;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

// ReSharper disable InconsistentNaming
namespace VisualHardpointLimits
{
    public static class VisualHardpointLimits
    {
        private static string ModName = "VisualHardpointLimits";

        internal static Configuration config;

        public static void Init()
        {
            config = new Configuration();
            config.Load();

            var harmony = HarmonyInstance.Create(ModName);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public static void Reset()
        {
        }

        public class Configuration
        {
            public bool allowLRMsInSmallerSlotsForAll = false;
            public string[] allowLRMsInSmallerSlotsForMechs = { "atlas" };
            public bool allowLRMsInLargerSlotsForAll = true;

            public string ModDirectory => Path.Combine(Path.GetDirectoryName(VersionManifestUtilities.MANIFEST_FILEPATH), @"..\..\..\Mods\VisualHardpointLimits\");
            public string ConfigPath => Path.Combine(ModDirectory, "Configuration.json");
            public string ManifestPath => Path.Combine(ModDirectory, "VersionManifest.csv");

            public void Load()
            {
                if (!File.Exists(ConfigPath))
                {
                    return;
                }

                string json;
                using (var reader = new StreamReader(ConfigPath))
                {
                    json = reader.ReadToEnd();
                }
                JSONSerializationUtility.FromJSON(this, json);
            }

            public void Save()
            {
                var json = JSONSerializationUtility.ToJSON(this);
                using (var writer = new StreamWriter(ConfigPath))
                {
                    writer.Write(json);
                }
            }
        }
    }

    // doesn't work as some weapon exchange code is running that ignore the result of ValidateAdd
    //[HarmonyPatch(typeof(MechLabLocationWidget), "ValidateAdd", new Type[] { typeof(MechComponentRef) })]
    //public static class BattleTech_MechLabLocationWidget_ValidateAdd_Patch
    //{
    //    static void Postfix(MechLabLocationWidget __instance, MechComponentRef newComponent, ref bool __result)
    //    {
    //        var vhl = new VHLMechLabLocationWidget(__instance);
    //        __result = vhl.CheckAvailableWeaponHardpoints(newComponent);
    //    }
    //}

    // used for loadout limitations
    [HarmonyPatch(typeof(MechLabLocationWidget), "OnMechLabDrop")]
    public static class BattleTechUI_MechLabLocationWidget_OnMechLabDrop_Patch
    {
        public static bool Prefix(MechLabLocationWidget __instance, PointerEventData eventData, MechLabDropTargetType addToType)
        {
            var vhl = new VHLMechLabLocationWidget(__instance);
            return vhl.MechLabLocationWidgetOnMechLabDrop(eventData);
        }
    }

    // used for better sorting
    [HarmonyPatch(typeof(MechRepresentationSimGame), "LoadWeapons")]
    public static class BattleTech_MechRepresentationSimGame_LoadWeapons_Patch
    {
        public static void Prefix(MechRepresentationSimGame __instance)
        {
        }

        public static void Postfix(MechRepresentationSimGame __instance)
        {
        }
    }

    // used for better sorting
    [HarmonyPatch(typeof(Mech), "InitGameRep")]
    public static class BattleTech_Mech_InitGameRep_Patch
    {
        public static void Prefix(Mech __instance, Transform parentTransform)
        {
        }

        public static void Postfix(Mech __instance, Transform parentTransform)
        {
        }
    }

    // used for better sorting
    [HarmonyPatch(typeof(MechDef), "RefreshInventory")]
    public static class BattleTech_MechDef_RefreshInventory_Patch
    {
        public static void Prefix(MechDef __instance)
        {
            /*
             mechComponentRef.RefreshComponentDef();
			if (!mechComponentRef.hasPrefabName && mechComponentRef.Def != null && this.Chassis != null && this.Chassis.HardpointDataDef != null)
			{
				mechComponentRef.prefabName = MechHardpointRules.GetComponentPrefabName(this.Chassis.HardpointDataDef, mechComponentRef, this.Chassis.PrefabBase, mechComponentRef.MountedLocation.ToString().ToLower(), ref list);
				mechComponentRef.hasPrefabName = true;
			}
             */
        }

        public static void Postfix(MechDef __instance)
        {
        }
    }
    
    // used for better sorting, here we should return the result that was cached before during better sorting
    [HarmonyPatch(typeof(MechHardpointRules), "GetComponentPrefabName")]
    public static class BattleTech_MechHardpointRules_GetComponentPrefabName_Patch
    {
        // ReSharper disable once RedundantAssignment
        public static bool Prefix(HardpointDataDef hardpointDataDef, BaseComponentRef componentRef, string prefabBase, string location, ref List<string> usedPrefabNames, ref string __result)
        {
            __result = VHLMechHardpointRules.GetComponentPrefabName(hardpointDataDef, componentRef, prefabBase, location, ref usedPrefabNames);
            return __result == null;
        }
    }

    // used for quickdraw patch
    [HarmonyPatch(typeof(VersionManifestUtilities), "LoadDefaultManifest")]
    public static class BattleTech_VersionManifestUtilities_LoadDefaultManifest_Patch
    {
        // ReSharper disable once RedundantAssignment
        public static void Postfix(ref VersionManifest __result)
        {
            var addendum = VersionManifestUtilities.ManifestFromCSV(VisualHardpointLimits.config.ManifestPath);
            foreach (var entry in addendum.Entries)
            {
                __result.AddOrUpdate(entry.Id, entry.FilePath, entry.Type, entry.AddedOn, entry.AssetBundleName,
                    entry.IsAssetBundlePersistent);
            }
        }
    }

    internal class VHLMechLabLocationWidget
    {
        private readonly MechLabLocationWidget _widget;
        private readonly MechLabLocationWidgetAdapter _widgetAdapter;

        internal VHLMechLabLocationWidget(MechLabLocationWidget widget)
        {
            _widget = widget;
            _widgetAdapter = new MechLabLocationWidgetAdapter(widget);
        }

        internal bool MechLabLocationWidgetOnMechLabDrop(PointerEventData eventData)
		{
            if (!_widgetAdapter.MechLab.Initialized)
            {
                return true;
            }
            if (_widgetAdapter.MechLab.DragItem == null)
            {
                return true;
            }
            var dragItem = _widgetAdapter.MechLab.DragItem;
            var componentRef = dragItem.ComponentRef;

            if (componentRef == null || componentRef.ComponentDefType != ComponentType.Weapon)
            {
                return true;
            }

            if (GetAvailableWeaponComponentPrefabName(componentRef) == null)
			{
                var dropErrorMessage = string.Format("Cannot add {0} to {1}: There are no available {2} hardpoints.",
                    componentRef.Def.Description.Name,
                     _widgetAdapter.LocationName.text,
                    componentRef.Def.PrefabIdentifier.ToUpper()
                    );
                _widgetAdapter.MechLab.ShowDropErrorMessage(dropErrorMessage);
                _widgetAdapter.MechLab.OnDrop(eventData);
                return false;
            }

			return true;
		}

		private string GetAvailableWeaponComponentPrefabName(MechComponentRef newComponentRef)
		{
		    var chassis = _widgetAdapter.MechLab.activeMechDef.Chassis;
            var hardpointDataDef = chassis.HardpointDataDef;
			var location = _widget.loadout.Location.ToString().ToLower();

		    var usedPrefabNames = new List<string>();

            _widgetAdapter.LocalInventory
                .Select(item => item.ComponentRef)
                .Where(component => component.ComponentDefType == ComponentType.Weapon)
                .Do(localComponentRef =>
            {
                var prefabName = VHLComponentLimitsCalculator.GetAvailableWeaponComponentPrefabName(hardpointDataDef, localComponentRef, chassis.PrefabBase, location, usedPrefabNames);
                usedPrefabNames.Add(prefabName);
            });

            return VHLComponentLimitsCalculator.GetAvailableWeaponComponentPrefabName(hardpointDataDef, newComponentRef, chassis.PrefabBase, location, usedPrefabNames);
        }

        internal class MechLabLocationWidgetAdapter
        {
            private readonly Traverse _traverse;

            internal MechLabLocationWidgetAdapter(MechLabLocationWidget widget)
            {
                _traverse = Traverse.Create(widget);
            }

            internal MechLabPanel MechLab
                => _traverse.Field("mechLab").GetValue() as MechLabPanel;

            internal TextMeshProUGUI LocationName
                => _traverse.Field("locationName").GetValue() as TextMeshProUGUI;

            internal List<MechLabItemSlotElement> LocalInventory
                => _traverse.Field("localInventory").GetValue() as List<MechLabItemSlotElement>;
        }
    }

    internal static class VHLMechHardpointRules
    {
        internal static string GetComponentPrefabName(HardpointDataDef hardpointDataDef, BaseComponentRef componentRef, string prefabBase, string location, ref List<string> usedPrefabNames)
        {
            var weaponDef = componentRef.Def as WeaponDef;
            if (weaponDef == null)
            {
                return null;
            }
            switch (weaponDef.Category)
            {
                case WeaponCategory.Ballistic:
                case WeaponCategory.Energy:
                case WeaponCategory.Missile:
                case WeaponCategory.AntiPersonnel:
                    break;
                default:
                    return null;
            }

            var foundPrefabName = VHLComponentLimitsCalculator.GetAvailableWeaponComponentPrefabName(hardpointDataDef, componentRef, prefabBase, location, usedPrefabNames);
            if (foundPrefabName == null)
            {
                return null;
            }
            usedPrefabNames.Add(foundPrefabName);
            return foundPrefabName;
        }
    }

    internal static class VHLComponentLimitsCalculator
    {
        internal static string GetAvailableWeaponComponentPrefabName(HardpointDataDef hardpointDataDef, BaseComponentRef componentRef, string prefabBase, string location, List<string> usedPrefabNames)
        {
            var usedPrefarbNamesArray = usedPrefabNames.ToArray();
            var availablePrefabNames = hardpointDataDef.HardpointData
                .Single(x => x.location == location) // only at location
                .weapons
                .Where(hpset => !hpset.Intersect(usedPrefarbNamesArray).Any()) // only include hardpoint sets not yet used
                .OrderBy(hpset => hpset.Length) // sort hardpoints by how many weapon types are supported (use up the ones with less options first)
                .SelectMany(hpset => hpset) // we don't care about groups anymore, just flatten everything into one stream
                .ToList();

            return GetAvailableWeaponComponentPrefabName(componentRef.Def.PrefabIdentifier.ToLower(), prefabBase, availablePrefabNames);
        }

        internal static string GetAvailableWeaponComponentPrefabName(string prefabId, string prefabBase, List<string> availablePrefabNames)
        {
            List<string> compatibleTerms;
            if (prefabId.Contains("lrm"))
            {
                var prefabIdFix = prefabId == "lrm20" ? "srm20" : prefabId; // yea sure, srm20 ...
                var order = new[] { "lrm5", "lrm10", "lrm15", "srm20" };
                var index = Array.IndexOf(order, prefabIdFix);

                compatibleTerms = new List<string> { prefabIdFix };

                if (VisualHardpointLimits.config.allowLRMsInLargerSlotsForAll)
                {
                    for (var i = index + 1; i < order.Length; i++)
                    {
                        compatibleTerms.Add(order[i]);
                    }
                }

                if (VisualHardpointLimits.config.allowLRMsInSmallerSlotsForAll || VisualHardpointLimits.config.allowLRMsInSmallerSlotsForMechs.Contains(prefabBase))
                {
                    for (var i = index - 1; i >= 0; i--)
                    {
                        compatibleTerms.Add(order[i]);
                    }
                }
            }
            else
            {
                compatibleTerms = new List<string> { prefabId };
            }

            foreach (var term in compatibleTerms)
            {
                var prefabName = availablePrefabNames.FirstOrDefault(x => x.Contains("_" + term + "_"));
                if (prefabName != null)
                {
                    return prefabName;
                }
            }
            return null;
        }
    }
}
