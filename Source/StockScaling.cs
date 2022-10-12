using System;
using RimWorld;
using TO.Mod;
using Verse;

namespace TO
{
	public static class StockScaling
	{
		public static double Calculate(TraderKindCategory category, ThingDef def, float? wealth)
		{
			var linearSetting = def == ThingDefOf.Silver
				? Settings.GetSilverScaling(category)
				: Settings.GetStockScaling(category);
			if (linearSetting == Settings.MinStockScaling)
			{
				linearSetting = 100;
			}

			var linearScaling = linearSetting / 100.0;

			var wealthScaling = Settings.GetWealthScaling(category);
			if (wealthScaling <= 0.0 || wealth == null) return linearScaling;
			var wealthScale = wealthScaling + wealth.Value;
			var logWealthScale = Math.Log(wealthScale, wealthScaling);
			// ToDo adjustment
			return linearScaling * logWealthScale;
		}
	}
}