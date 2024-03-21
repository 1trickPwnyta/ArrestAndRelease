using RimWorld;
using System.Linq;
using Verse;
using Verse.AI;

namespace ArrestAndRelease
{
    public static class RoomExtensions
    {
        public static bool IsSuitableForArrest(this Room room, Pawn arrester, Pawn arrestee)
        {
            if (room.Fogged || !Building_Bed.RoomCanBePrisonCell(room))
            {
                return false;
            }

            if (room.ContainedBeds.Any((bed) => 
                    bed.Faction == Faction.OfPlayer && 
                    (bed.AnyUnoccupiedSleepingSlot || bed.CurOccupants.Contains(arrestee)) && 
                    bed.def.building.bed_humanlike &&
                    arrester.CanReach(bed, PathEndMode.OnCell, Danger.Deadly)))
            {
                return true;
            }

            foreach (IntVec3 cell in room.Cells)
            {
                if (canPlaceSleepingSpot(room.Map, cell) && arrester.CanReach(cell, PathEndMode.OnCell, Danger.Deadly))
                {
                    foreach (IntVec3 adj in GenAdj.CardinalDirections)
                    {
                        if (canPlaceSleepingSpot(room.Map, cell + adj))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private static bool canPlaceSleepingSpot(Map map, IntVec3 cell)
        {
            Building edifice = cell.GetEdifice(map);
            if (edifice != null && edifice.GetStatValue(StatDefOf.WorkToBuild, true) > 0f)
            {
                return false;
            }

            if (!cell.GetTerrain(map).affordances.Contains(TerrainAffordanceDefOf.Light))
            {
                return false;
            }

            return true;
        }
    }
}
