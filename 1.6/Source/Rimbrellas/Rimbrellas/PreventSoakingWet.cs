using System.Reflection;
using RimWorld;
using Verse;
using Verse.AI;
using HarmonyLib;

namespace Umbrellas
{
	[HarmonyPatch(typeof(Pawn_MindState), "MindStateTickInterval")]
	class RBMindStatePrefix
	{
		static bool Prefix(Pawn_MindState __instance, ref int delta)
		{
			if (!__instance.pawn.NonHumanlikeOrWildMan() && (UmbrellaDefMethods.HasUmbrella(__instance.pawn) || (RimbrellasMod.settings.cowboyHatsPreventSoakingWet && UmbrellaDefMethods.HasCowboyHat(__instance.pawn))))
			{
				MethodInfo CanGainGainThoughtNow = AccessTools.Method(typeof(Verse.AI.Pawn_MindState), "CanGainGainThoughtNow");

				if (__instance.wantsToTradeWithColony)
				{
					TradeUtility.CheckInteractWithTradersTeachOpportunity(__instance.pawn);
				}
				if (__instance.meleeThreat != null && !__instance.MeleeThreatStillThreat)
				{
					__instance.meleeThreat = null;
				}
				__instance.mentalStateHandler.MentalStateHandlerTickInterval(delta);
				__instance.mentalBreaker.MentalBreakerTickInterval(delta);
				__instance.mentalFitGenerator.TickInterval(delta);
				__instance.inspirationHandler.InspirationHandlerTickInterval(delta);
				if (!__instance.pawn.GetPosture().Laying())
				{
					__instance.applyBedThoughtsTick = 0;
				}
				if (__instance.pawn.IsHashIntervalTick(100, delta))
				{
					if (__instance.pawn.Spawned)
					{
						int regionsToScan = (__instance.anyCloseHostilesRecently ? 24 : 18);
						__instance.anyCloseHostilesRecently = PawnUtility.EnemiesAreNearby(__instance.pawn, regionsToScan, passDoors: true);
					}
					else
					{
						__instance.anyCloseHostilesRecently = false;
					}
				}
				if (__instance.WillJoinColonyIfRescued && __instance.AnythingPreventsJoiningColonyIfRescued)
				{
					__instance.WillJoinColonyIfRescued = false;
				}
				if (__instance.pawn.Spawned && __instance.pawn.IsWildMan() && !__instance.WildManEverReachedOutside && __instance.pawn.GetDistrict() != null && __instance.pawn.GetDistrict().TouchesMapEdge)
				{
					__instance.WildManEverReachedOutside = true;
				}
				if (__instance.pawn.Spawned && __instance.pawn.RaceProps.IsFlesh && __instance.pawn.needs.mood != null && __instance.pawn.IsHashIntervalTick(120, delta))
				{
					TerrainDef terrain = __instance.pawn.Position.GetTerrain(__instance.pawn.Map);
					if ((bool)CanGainGainThoughtNow.Invoke(__instance, new object[] {terrain.traversedThought}))
					{
						__instance.pawn.needs.mood.thoughts.memories.TryGainMemoryFast(terrain.traversedThought);
					}
					WeatherDef curWeatherLerped = __instance.pawn.Map.weatherManager.CurWeatherLerped;
					if ((bool)CanGainGainThoughtNow.Invoke(__instance, new object[] {curWeatherLerped.weatherThought}))
					{
						bool flag = __instance.pawn.Position.Roofed(__instance.pawn.Map);
						bool flag2 = curWeatherLerped.weatherThought.stages.Count == 1;
						if (!flag || !flag2)
						{
							int stage = ((!(flag2 || flag)) ? 1 : 0);
							// This code would assign the Soaking Wet thought to a pawn outside in the rain, but we've already checked that the pawn has an umbrella
							// __instance.pawn.needs.mood.thoughts.memories.TryGainMemoryFast(curWeatherLerped.weatherThought, stage);
						}
					}
					if (__instance.pawn.Position.GasDensity(__instance.pawn.Map, GasType.RotStink) > 0)
					{
						__instance.lastRotStinkTick = Find.TickManager.TicksGame;
					}
				}
				if (__instance.droppedWeapon != null && !__instance.droppedWeapon.Spawned)
				{
					__instance.droppedWeapon = null;
				}
				int num = GenLocalDate.DayTick(__instance.pawn);
				if (num < __instance.lastDayInteractionTick)
				{
					__instance.interactionsToday = 0;
				}
				__instance.lastDayInteractionTick = num;
				if ((__instance.pawn.IsFighting() && __instance.pawn.CurJob?.def != JobDefOf.Wait_Combat) || __instance.pawn.equipment?.Primary != null)
				{
					__instance.lastCombatantTick = Find.TickManager.TicksGame;
				}
				if (__instance.enemyTarget is Pawn pawn && pawn.mindState != null)
				{
					pawn.mindState.lastCombatantTick = Find.TickManager.TicksGame;
				}
				return false;
			}
			return true;
		}

		// Old version, before Tick changed to TickInterval
		//   static bool Prefix(Pawn_MindState __instance) {
		//       if (!__instance.pawn.NonHumanlikeOrWildMan() && (UmbrellaDefMethods.HasUmbrella(__instance.pawn) || (RimbrellasMod.settings.cowboyHatsPreventSoakingWet && UmbrellaDefMethods.HasCowboyHat(__instance.pawn)))) {
		// 	if (__instance.wantsToTradeWithColony) {
		// 		TradeUtility.CheckInteractWithTradersTeachOpportunity(__instance.pawn);
		// 	}
		// 	if (__instance.meleeThreat != null && !__instance.meleeThreatStillThreat) {
		// 		__instance.meleeThreat = null;
		// 	}
		// 	__instance.mentalStateHandler.__instance.mentalStateHandlerTick();
		// 	__instance.mentalBreaker.MentalBreakerTick();
		// 	__instance.inspirationHandler.__instance.inspirationHandlerTick();
		// 	if (!__instance.pawn.GetPosture().Laying()) {
		// 		__instance.applyBedThoughtsTick = 0;
		// 	}
		// 	if (__instance.pawn.IsHashIntervalTick(100)) {
		// 		if (__instance.pawn.Spawned) {
		// 			int regionsToScan = __instance.anyCloseHostilesRecently ? 24 : 18;
		// 			__instance.anyCloseHostilesRecently = PawnUtility.EnemiesAreNearby(__instance.pawn, regionsToScan, passDoors: true);
		// 		}
		// 		else {
		// 			__instance.anyCloseHostilesRecently = false;
		// 		}
		// 	}
		// 	if (__instance.WillJoinColonyIfRescued && __instance.AnythingPreventsJoiningColonyIfRescued) {
		// 		__instance.WillJoinColonyIfRescued = false;
		// 	}
		// 	if (__instance.pawn.Spawned && __instance.pawn.IsWildMan() && !__instance.WildManEverReachedOutside && __instance.pawn.GetRoom() != null && __instance.pawn.GetRoom().TouchesMapEdge) {
		// 		__instance.WildManEverReachedOutside = true;
		// 	}
		// 	if (Find.TickManager.TicksGame % 123 == 0 && __instance.pawn.Spawned && __instance.pawn.RaceProps.IsFlesh && __instance.pawn.needs.mood != null) {
		// 		__instance.pawn.Drawer.renderer.renderTree.SetDirty();

		// 		TerrainDef terrain = __instance.pawn.Position.GetTerrain(__instance.pawn.Map);
		// 		if (terrain.traversedThought != null) {
		// 			__instance.pawn.needs.mood.thoughts.memories.TryGainMemoryFast(terrain.traversedThought);
		// 		}
		// 		/*WeatherDef curWeatherLerped = __instance.pawn.Map.weatherManager.CurWeatherLerped;
		// 		if (curWeatherLerped.exposedThought != null && !__instance.pawn.Position.Roofed(__instance.pawn.Map)) {
		// 			__instance.pawn.needs.mood.thoughts.memories.TryGainMemoryFast(curWeatherLerped.exposedThought);
		// 		}*/ //this code would assign the Soaking Wet thought to a pawn outside in the rain, but we've already checked that the pawn has an umbrella
		// 	}
		// 	if (GenLocalDate.DayTick(__instance.pawn) == 0) {
		// 		__instance.interactionsToday = 0;
		// 	}
		// 	return false;
		//       }
		// return true;
		//   }
	}
}
