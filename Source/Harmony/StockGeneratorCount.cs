using System;
using HarmonyLib;
using RimWorld;
using TO.Mod;
using Verse;

namespace TO.Harmony
{
	[HarmonyPatch]
	public static class StockGeneratorCount
	{
		[HarmonyPostfix]
		[HarmonyPatch(typeof(StockGenerator), "RandomCountOf")]
		private static void ModifyGeneratedAmounts(ref StockGenerator __instance, ref int __result, ThingDef def)
		{
			var category = Category.Get(__instance.trader);
			var scaling = def == ThingDefOf.Silver ? Settings.GetSilverScaling(category) : Settings.GetStockScaling(category);
			__result = (int) (scaling * __result / 100.0f);

			if (!Settings.GetWealthScaling(category)) return;
			const double logBase = 100.0;
			var wealthScale = logBase + Find.World.PlayerWealthForStoryteller;
			var logWealthScale = Math.Log(wealthScale, logBase);
			// Make wealth scaling close to 1 with the starting wealth of the vanilla crashlanded/tribal scenarios.
			var adjustedLogWealthScale = 0.5 * logWealthScale;
			__result = (int) (__result * adjustedLogWealthScale);
		}
	}
}