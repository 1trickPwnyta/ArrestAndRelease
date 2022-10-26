using Verse;

namespace ArrestAndRelease
{
    public static class ArrestAndReleaseUtil
    {
        public static bool IsUnderArrestAndRelease(Pawn pawn)
        {
            Pawn carrier = pawn.CarriedBy;
            if (carrier != null)
            {
                if (carrier.CurJobDef == JobDefOf.ArrestAndRelease)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
