using RimWorld;
using Verse;
using Verse.AI;
using HarmonyLib;

namespace Umbrellas {
    public class UmbrellasPatcher {
        public static void DoPatching() {
            var harmony = new Harmony("com.battlemage64.umbrellas");
            harmony.PatchAll();
        }
    }
    [HarmonyPatch(typeof(Pawn_MindState),"MindStateTick")]
    class MindStatePatch {
        //static AccessTools.FieldRef<Pawn_MindState, Pawn> pawnRef = AccessTools.FieldRefAccess<Pawn_MindState, Pawn>("pawn");

        static void Postfix(Pawn_MindState __instance) {
            if (!__instance.pawn.NonHumanlikeOrWildMan()) { // probably inefficient but whatever
                if (Find.TickManager.TicksGame % 123 == 0 && __instance.pawn.Spawned && __instance.pawn.RaceProps.IsFlesh && __instance.pawn.needs.mood != null) { // same logic as original function
                    //if (__instance.pawn.needs.mood.thoughts.memories.GetFirstMemoryOfDef(ThoughtDef.Named("SoakingWet")) != null) Log.Message(__instance.pawn.Map.terrainGrid.TerrainAt.ToString());
                    if (__instance.pawn.needs.mood.thoughts.memories.GetFirstMemoryOfDef(ThoughtDef.Named("SoakingWet")) != null && !__instance.pawn.Map.terrainGrid.TerrainAt(__instance.pawn.Position).IsWater) {
                        // statement above: if the pawn has a Soaking Wet thought and the terrain at its position is not water
                        //next: check if pawn has an umbrella
                        if (UmbrellaDefMethods.HasUmbrella(__instance.pawn)) {
                            __instance.pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDef.Named("SoakingWet"));
                        }
                    }
                }
            }
        }
    }
}
