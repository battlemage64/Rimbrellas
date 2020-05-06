using RimWorld;
using Verse;
using Verse.AI;
using HarmonyLib;

namespace Umbrellas {
    [HarmonyPatch(typeof(Pawn_MindState),"MindStateTick")]
    class MindStatePatch {

        static void Postfix(Pawn_MindState __instance) {
            if (!__instance.pawn.NonHumanlikeOrWildMan()) { // probably inefficient but whatever
                if (Find.TickManager.TicksGame % 123 == 0 && __instance.pawn.Spawned && __instance.pawn.RaceProps.IsFlesh && __instance.pawn.needs.mood != null) { // same logic as original function
                    if (__instance.pawn.needs.mood.thoughts.memories.GetFirstMemoryOfDef(ThoughtDef.Named("SoakingWet")) != null && !__instance.pawn.Map.terrainGrid.TerrainAt(__instance.pawn.Position).IsWater) {
                        // statement above: if the pawn has a Soaking Wet thought and the terrain at its position is not water
                        //next: check if pawn has an umbrella
                        if (UmbrellaDefMethods.HasUmbrella(__instance.pawn)) {
                            __instance.pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDef.Named("SoakingWet"));
                        }
                    }
                    if (!(RimbrellasMod.settings.showUmbrellasWhenInside) && Find.TickManager.TicksGame % 246 == 0) {
                        //this should run about once per second (could later be lowered to improve performance) and all it does is call this function in case the pawn has gone inside
                        __instance.pawn.Drawer.renderer.graphics.ResolveApparelGraphics();
                    }
                }
            }
        }
    }
}
