using System;
using RimWorld;
using TO.Mod;
using Verse;

namespace TO
{
	public static class StockScaling
	{
		public static double Calculate(TraderKindCategory category, ThingDef def)
		{
			var linearSetting = def == ThingDefOf.Silver
				? Settings.GetSilverScaling(category)
				: Settings.GetStockScaling(category);
			if (linearSetting == Settings.MinStockScaling)
			{
				linearSetting = 100;
			}

			var linearScaling = linearSetting / 100.0;
			
			if (!Settings.GetWealthScaling(category)) return linearScaling;
			const double logBase = 100.0;
			var wealthScale = logBase + Find.World.PlayerWealthForStoryteller;
			var logWealthScale = Math.Log(wealthScale, logBase);
			// Make wealth scaling close to 1 with the starting wealth of the vanilla crashlanded / tribal scenarios.
			var adjustedLogWealthScale = 0.5 * logWealthScale;
			return linearScaling * adjustedLogWealthScale;
		}
	}
}