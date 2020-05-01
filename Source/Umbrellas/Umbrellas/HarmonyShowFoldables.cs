using RimWorld;
using Verse;
using Verse.AI;
using HarmonyLib;
using System.Collections.Generic;
using System.Diagnostics;

namespace Umbrellas {
    [HarmonyPatch(typeof(WeatherManager),"TransitionTo")]
    class HarmonyOnWeatherChange {
        static void Postfix(ref WeatherManager __instance, Map ___map) {
            foreach (Pawn pawn in ___map.mapPawns.AllPawnsSpawned) {
                if (!pawn.AnimalOrWildMan()) {
                    pawn.Drawer.renderer.graphics.ResolveApparelGraphics();
                    PortraitsCache.SetDirty(pawn);
                }
            }
        }
    }

    [HarmonyPatch(typeof(PawnGraphicSet), "ResolveApparelGraphics")]
    class HarmonyShowFoldables {
        static void Postfix(ref PawnGraphicSet __instance) {
            if (!__instance.pawn.Spawned) return;
            //Log.Message(__instance.pawn.Map.weatherManager.curWeather.label);
            if (__instance.pawn.Map.weatherManager.curWeather.rainRate == 0) {
                List<ApparelGraphicRecord> remove = new List<ApparelGraphicRecord>();
                foreach (ApparelGraphicRecord rec in __instance.apparelGraphics) {
                    if (UmbrellaDefMethods.HideableUmbrellaDefs.Contains(rec.sourceApparel.def)) {
                        remove.Add(rec);
                    }
                }
                foreach (ApparelGraphicRecord rec in remove) {
                    __instance.apparelGraphics.Remove(rec);
                }

            }
        }
    }
}
//if (map.weatherManager.RainRate > 0) {