using RimWorld;
using Verse;

namespace MyCustomBedMod
{
    public class ThoughtWorker_CowBed_Slave : ThoughtWorker
    {
        protected override ThoughtState CurrentStateInternal(Pawn p)
        {
            if (!p.Spawned || p.CurrentBed() == null || !p.IsSlave) return ThoughtState.Inactive;
            Building_Bed bed = p.CurrentBed();
            if (bed.SleepingSlotsCount <= 1) return ThoughtState.Inactive;

            if (!p.health.hediffSet.HasHediff(HediffDef.Named("pet_cow"))) return ThoughtState.Inactive;

            foreach (Pawn partner in bed.CurOccupants)
            {
                if (partner == p) continue;
                if (LovePartnerRelationUtility.LovePartnerRelationExists(p, partner)) continue;
                if (partner.IsFreeColonist) return ThoughtState.ActiveDefault;
            }
            return ThoughtState.Inactive;
        }
    }
}