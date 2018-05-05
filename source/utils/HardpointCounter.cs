using BattleTech;

namespace VisualHardpointLimits
{
    internal class HardpointCounter
    {
        internal readonly int numBallistic;
        internal readonly int numEnergy;
        internal readonly int numMissile;
        internal readonly int numSmall;

        internal HardpointCounter(ChassisDef chassisDef, ChassisLocations location)
        {
            MechStatisticsRules.GetHardpointCountForLocation(chassisDef, location, ref numBallistic, ref numEnergy, ref numMissile, ref numSmall);
        }
    }
}