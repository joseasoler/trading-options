using System;
using RimWorld;
using TO.Mod;
using Verse;

namespace TradingOptions
{
	public static class StockScaling
	{
		/// <summary>
		/// Last wealth for which scaling was calculated
		/// </summary>
		private static float _lastWealth = -1.0f;

		/// <summary>
		/// Last wealth scaling value used to calculate full scaling.
		/// </summary>
		private static double _lastWealthScaling = -1.0f;

		/// <summary>
		/// Cached adjustment for the last wealth.
		/// </summary>
		private static double _adjustedLogWealthScale;

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

			if (Math.Abs(wealth.Value - _lastWealth) < 0.1f && Math.Abs(wealthScaling - _lastWealthScaling) < 0.000001f)
			{
				return linearScaling * _adjustedLogWealthScale;
			}

			_lastWealthScaling = wealthScaling;
			var wealthScale = wealthScaling + wealth.Value;
			var logWealthScale = Math.Log(wealthScale, wealthScaling);
			var wealthScaleAdjustment = Math.Log(wealthScaling + 10000f, wealthScaling);
			_lastWealth = wealth.Value;
			_adjustedLogWealthScale = Math.Max(1.0, 1.0 + logWealthScale - wealthScaleAdjustment);

			return linearScaling * _adjustedLogWealthScale;
		}
	}
}