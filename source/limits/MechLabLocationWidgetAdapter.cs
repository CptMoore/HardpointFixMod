using System.Collections.Generic;
using BattleTech;
using BattleTech.UI;
using Harmony;
using TMPro;

namespace VisualHardpointLimits
{
    internal class MechLabLocationWidgetAdapter
    {
        private readonly MechLabLocationWidget _instance;
        private readonly Traverse _traverse;

        internal MechLabLocationWidgetAdapter(MechLabLocationWidget instance)
        {
            _instance = instance;
            _traverse = Traverse.Create(instance);
        }

        internal MechLabPanel MechLab
        {
            get { return _traverse.Field("mechLab").GetValue() as MechLabPanel; }
        }

        internal TextMeshProUGUI LocationName
        {
            get { return _traverse.Field("locationName").GetValue() as TextMeshProUGUI; }
        }

        internal List<MechLabItemSlotElement> LocalInventory
        {
            get { return _traverse.Field("localInventory").GetValue() as List<MechLabItemSlotElement>; }
        }

        internal LocationLoadoutDef Loadout
        {
            get { return _instance.loadout; }
        }
    }
}