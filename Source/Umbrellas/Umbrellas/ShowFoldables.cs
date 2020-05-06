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
            if (!(RimbrellasMod.settings.showUmbrellasInRain || RimbrellasMod.settings.showUmbrellasInSnow)) return;
            if (!(ShowFoldablesNow(__instance.pawn.Map)) || (__instance.pawn.Map.roofGrid.Roofed(__instance.pawn.Position) && !RimbrellasMod.settings.showUmbrellasWhenInside)) {
                List<ApparelGraphicRecord> remove = new List<ApparelGraphicRecord>();
                foreach (ApparelGraphicRecord rec in __instance.apparelGraphics) {
                    if (UmbrellaDefMethods.HideableUmbrellaDefs.Contains(rec.sourceApparel.def)) {
                        remove.Add(rec);
                    }
                }
                foreach (ApparelGraphicRecord rec in remove) {
                    __instance.apparelGraphics.Remove(rec);
                }
                PortraitsCache.SetDirty(__instance.pawn); // NOTE: this is not necessarily required for when the weather changes and could be moved to only call when checking for roof above
            }
        }
        private static bool ShowFoldablesNow(Map map) {
            if (map.weatherManager.curWeather.rainRate == 0) {
                return false;
            }
            if (RimbrellasMod.settings.showUmbrellasInSnow && map.weatherManager.curWeather.snowRate > 0) {
                return true;
            }
            if (RimbrellasMod.settings.showUmbrellasInRain && map.weatherManager.curWeather.snowRate == 0) { // it's already been determined that rainRate > 0 because if it was 0 it would have returned false above
                return true;
            }
            return false;
        }
    }
}
//if (map.weatherManager.RainRate > 0) {