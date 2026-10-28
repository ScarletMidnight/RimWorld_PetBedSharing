using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace MyCustomBedMod
{
    [HarmonyPatch(typeof(Toils_LayDown), "ApplyBedThoughts")]
    public static class Patch_SleptInBedWithSomeone
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn actor, Building_Bed bed)
        {
            if (actor == null || !actor.Spawned || bed == null || bed.SleepingSlotsCount <= 1) return;

            foreach (Pawn partner in bed.CurOccupants)
            {
                if (partner == actor) continue;

                // Case 1: Master waking up next to Pet Slave
                if (actor.IsFreeColonist && partner.IsSlave && IsPetSlave(partner))
                {
                    PurgeBedSharingMemories(actor);

                    ThoughtDef masterThought = GetMasterThoughtDef(partner);
                    if (masterThought != null)
                    {
                        actor.needs?.mood?.thoughts?.memories?.TryGainMemory(masterThought);
                    }
                    return;
                }

                // Case 2: Pet Slave waking up next to Master
                if (actor.IsSlave && IsPetSlave(actor) && partner.IsFreeColonist)
                {
                    PurgeBedSharingMemories(actor);

                    ThoughtDef slaveThought = GetSlaveThoughtDef(actor);
                    if (slaveThought != null)
                    {
                        actor.needs?.mood?.thoughts?.memories?.TryGainMemory(slaveThought);
                    }
                    return;
                }
            }
        }

        private static void PurgeBedSharingMemories(Pawn pawn)
        {
            var memories = pawn?.needs?.mood?.thoughts?.memories;
            if (memories == null) return;

            List<Thought_Memory> toRemove = new List<Thought_Memory>();
            foreach (Thought_Memory mem in memories.Memories)
            {
                if (mem?.def?.defName != null)
                {
                    string name = mem.def.defName;

                    if (name.StartsWith("SleptInBed") || 
                        name.Contains("BedWith") || 
                        name.Contains("SleptInRoom") ||
                        name == "SharedBed")
                    {
                        toRemove.Add(mem);
                    }
                }
            }

            foreach (var mem in toRemove)
            {
                memories.RemoveMemory(mem);
            }
        }

        private static ThoughtDef GetSlaveThoughtDef(Pawn slave)
        {
            var hediffs = slave.health?.hediffSet;
            if (hediffs == null) return null;

            if (hediffs.HasHediff(HediffDef.Named("pet_doggy"))) return ThoughtDef.Named("PetDoggy_SharedBed_Slave");
            if (hediffs.HasHediff(HediffDef.Named("pet_kitty"))) return ThoughtDef.Named("PetKitty_SharedBed_Slave");
            if (hediffs.HasHediff(HediffDef.Named("pet_cow")))   return ThoughtDef.Named("PetCow_SharedBed_Slave");
            if (hediffs.HasHediff(HediffDef.Named("pet_doll")))  return ThoughtDef.Named("PetDoll_SharedBed_Slave");
            if (hediffs.HasHediff(HediffDef.Named("pet_pony")))  return ThoughtDef.Named("PetPony_SharedBed_Slave");

            return null;
        }

        private static ThoughtDef GetMasterThoughtDef(Pawn slave)
        {
            var hediffs = slave.health?.hediffSet;
            if (hediffs == null) return null;

            if (hediffs.HasHediff(HediffDef.Named("pet_doggy"))) return ThoughtDef.Named("PetDoggy_SharedBed_Master");
            if (hediffs.HasHediff(HediffDef.Named("pet_kitty"))) return ThoughtDef.Named("PetKitty_SharedBed_Master");
            if (hediffs.HasHediff(HediffDef.Named("pet_cow")))   return ThoughtDef.Named("PetCow_SharedBed_Master");
            if (hediffs.HasHediff(HediffDef.Named("pet_doll")))  return ThoughtDef.Named("PetDoll_SharedBed_Master");
            if (hediffs.HasHediff(HediffDef.Named("pet_pony")))  return ThoughtDef.Named("PetPony_SharedBed_Master");

            return null;
        }

        private static bool IsPetSlave(Pawn pawn)
        {
            return GetSlaveThoughtDef(pawn) != null;
        }
    }
}