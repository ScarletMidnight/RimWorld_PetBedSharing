using RimWorld;
using Verse;

namespace MyCustomBedMod
{
    public class ThoughtWorker_KittyBed_Master : ThoughtWorker
    {
        protected override ThoughtState CurrentStateInternal(Pawn p)
        {
            if (!p.Spawned || p.CurrentBed() == null || !p.IsFreeColonist) return ThoughtState.Inactive;
            Building_Bed bed = p.CurrentBed();
            if (bed.SleepingSlotsCount <= 1) return ThoughtState.Inactive;

            HediffDef targetHediff = HediffDef.Named("pet_kitty");
            foreach (Pawn partner in bed.CurOccupants)
            {
                if (partner == p) continue;
                if (partner.IsSlave && partner.health.hediffSet.HasHediff(targetHediff))
                {
                    return ThoughtState.ActiveDefault;
                }
            }
            return ThoughtState.Inactive;
        }
    }
}