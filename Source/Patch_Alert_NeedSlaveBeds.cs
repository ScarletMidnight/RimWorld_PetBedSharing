using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace ScarletMidnight.slavepetbedthoughts
{
    [HarmonyPatch(typeof(Alert_NeedSlaveBeds), "GetReport")]
    public static class Patch_Alert_NeedSlaveBeds
    {
        public static bool Prefix(ref AlertReport __result)
        {
            if (Find.Maps == null)
            {
                __result = AlertReport.Inactive;
                return false;
            }

            foreach (Map map in Find.Maps)
            {
                if (map == null || !map.IsPlayerHome)
                {
                    continue;
                }

                List<Pawn> slaves = map.mapPawns?.SlavesOfColonySpawned;
                if (slaves == null)
                {
                    continue;
                }

                List<Pawn> unassignedSlaves = new List<Pawn>();

                foreach (Pawn pawn in slaves)
                {
                    if (pawn != null && !pawn.Dead && pawn.IsSlave)
                    {
                        // Check if the slave owns ANY bed slot (Colonist bed, Slave bed, or Pet Bed)
                        Building_Bed ownedBed = pawn.ownership?.OwnedBed;
                        if (ownedBed == null)
                        {
                            unassignedSlaves.Add(pawn);
                        }
                    }
                }

                if (unassignedSlaves.Count > 0)
                {
                    List<GlobalTargetInfo> culprits = unassignedSlaves.Select(p => new GlobalTargetInfo(p)).ToList();
                    __result = AlertReport.CulpritsAre(culprits);
                    return false; // Skip vanilla logic
                }
            }

            __result = AlertReport.Inactive;
            return false; // Skip vanilla logic
        }
    }
}