using HarmonyLib;
using RimWorld;
using Verse;

namespace MyCustomBedMod
{
    // Suppresses the situational "Sharing bed" debuff specifically for Master and Pet Slave bed partners
    [HarmonyPatch(typeof(ThoughtWorker_SharedBed), "CurrentStateInternal")]
    public static class Patch_ThoughtWorker_SharedRemoval
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn p, ref ThoughtState __result)
        {
            if (!__result.Active || p == null || !p.Spawned) return;

            Building_Bed bed = p.CurrentBed() ?? p.ownership?.OwnedBed;
            if (bed == null || bed.SleepingSlotsCount <= 1) return;

            var owners = bed.OwnersForReading;
            if (owners != null)
            {
                foreach (Pawn partner in owners)
                {
                    if (partner == p || partner == null) continue;
                    if (IsPetMasterPair(p, partner))
                    {
                        __result = ThoughtState.Inactive;
                        return;
                    }
                }
            }

            foreach (Pawn partner in bed.CurOccupants)
            {
                if (partner == p || partner == null) continue;
                if (IsPetMasterPair(p, partner))
                {
                    __result = ThoughtState.Inactive;
                    return;
                }
            }
        }

        private static bool IsPetMasterPair(Pawn p1, Pawn p2)
        {
            if (p1.IsFreeColonist && p2.IsSlave && IsPetSlave(p2)) return true;
            if (p1.IsSlave && IsPetSlave(p1) && p2.IsFreeColonist) return true;
            return false;
        }

        private static bool IsPetSlave(Pawn pawn)
        {
            var hediffs = pawn?.health?.hediffSet;
            if (hediffs == null) return false;

            return hediffs.HasHediff(HediffDef.Named("pet_doggy")) ||
                   hediffs.HasHediff(HediffDef.Named("pet_kitty")) ||
                   hediffs.HasHediff(HediffDef.Named("pet_cow"))   ||
                   hediffs.HasHediff(HediffDef.Named("pet_doll"))  ||
                   hediffs.HasHediff(HediffDef.Named("pet_pony"));
        }
    }
}