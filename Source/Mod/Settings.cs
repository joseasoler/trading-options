using System.Collections.Generic;
using Verse;

namespace TO.Mod
{
	internal class SettingValues
	{
		/// <summary>
		/// Time value for changing trader frequency. The meaning of this value depends on each trader.
		/// Zero always means "let the storyteller decide".
		/// </summary>
		public Dictionary<TraderKindCategory, int> FrequencyTime = new Dictionary<TraderKindCategory, int>
		{
			{TraderKindCategory.Orbital, 0},
			{TraderKindCategory.Caravan, 0},
			{TraderKindCategory.Visitor, 0}
		};

		/// <summary>
		/// Amount value for changing trader frequency. The meaning of this value depends on each trader.
		/// Zero always means "let the storyteller decide".
		/// </summary>
		public Dictionary<TraderKindCategory, int> FrequencyAmount = new Dictionary<TraderKindCategory, int>
		{
			{TraderKindCategory.Orbital, 0},
			{TraderKindCategory.Caravan, 0},
			{TraderKindCategory.Visitor, 0}
		};

		/// <summary>
		/// % factor applied to trader frequency chance when using random storytellers. 
		/// Zero always means "let the storyteller decide".
		/// </summary>
		public Dictionary<TraderKindCategory, int> FrequencyChanceFactor = new Dictionary<TraderKindCategory, int>
		{
			{TraderKindCategory.Orbital, 100},
			{TraderKindCategory.Caravan, 100}
		};

		public const int MinScaling = 24;

		/// <summary>
		/// Silver stock of each trader category in %.
		/// </summary>
		public Dictionary<TraderKindCategory, int> SilverScaling = new Dictionary<TraderKindCategory, int>
		{
			{TraderKindCategory.Orbital, MinScaling},
			{TraderKindCategory.Settlement, MinScaling},
			{TraderKindCategory.Caravan, MinScaling},
			{TraderKindCategory.Visitor, MinScaling}
		};

		/// <summary>
		/// Other stock of each trader category in %.
		/// </summary>
		public Dictionary<TraderKindCategory, int> StockScaling = new Dictionary<TraderKindCategory, int>
		{
			{TraderKindCategory.Orbital, MinScaling},
			{TraderKindCategory.Settlement, MinScaling},
			{TraderKindCategory.Caravan, MinScaling},
			{TraderKindCategory.Visitor, MinScaling}
		};

		/// <summary>
		/// Scale trader stock by colony wealth
		/// </summary>
		public Dictionary<TraderKindCategory, bool> WealthScaling = new Dictionary<TraderKindCategory, bool>
		{
			{TraderKindCategory.Orbital, false},
			{TraderKindCategory.Settlement, false},
			{TraderKindCategory.Caravan, false},
			{TraderKindCategory.Visitor, false}
		};
	}

	/// <summary>
	/// Handles mod settings.
	/// </summary>
	public class Settings : ModSettings
	{
		/// <summary>
		/// Current values for all settings.
		/// </summary>
		private static SettingValues _values = new SettingValues();

		public static int GetFrequencyTime(TraderKindCategory category)
		{
			return _values.FrequencyTime[category];
		}

		public static void SetFrequencyTime(TraderKindCategory category, int value)
		{
			_values.FrequencyTime[category] = value;
		}

		public static int GetFrequencyAmount(TraderKindCategory category)
		{
			return _values.FrequencyAmount[category];
		}

		public static void SetFrequencyAmount(TraderKindCategory category, int value)
		{
			_values.FrequencyAmount[category] = value;
		}

		public static int GetFrequencyChanceFactor(TraderKindCategory category)
		{
			return category == TraderKindCategory.Visitor
				? _values.FrequencyChanceFactor[TraderKindCategory.Caravan]
				: _values.FrequencyChanceFactor[category];
		}

		public static void SetFrequencyChanceFactor(TraderKindCategory category, int value)
		{
			if (category == TraderKindCategory.Visitor)
			{
				_values.FrequencyChanceFactor[TraderKindCategory.Caravan] = value;
			}
			else
			{
				_values.FrequencyChanceFactor[category] = value;
			}
		}

		public static int GetSilverScaling(TraderKindCategory category)
		{
			return _values.SilverScaling[category];
		}

		public static void SetSilverScaling(TraderKindCategory category, int value)
		{
			_values.SilverScaling[category] = value;
		}

		public static int GetStockScaling(TraderKindCategory category)
		{
			return _values.StockScaling[category];
		}

		public static void SetStockScaling(TraderKindCategory category, int value)
		{
			_values.StockScaling[category] = value;
		}

		/// <summary>
		/// Minimum allowed value for StockScaling and SilverScaling settings in %.
		/// </summary>
		public const int MinStockScaling = SettingValues.MinScaling;

		/// <summary>
		/// Maximum allowed value for StockScaling and SilverScaling settings in %.
		/// </summary>
		public const int MaxStockScaling = 500;

		public static bool GetWealthScaling(TraderKindCategory category)
		{
			return _values.WealthScaling[category];
		}

		public static void SetWealthScaling(TraderKindCategory category, bool value)
		{
			_values.WealthScaling[category] = value;
		}

		public static void Reset()
		{
			_values = new SettingValues();
		}

		/// <summary>
		/// Save and load preferences.
		/// </summary>
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Collections.Look(ref _values.FrequencyTime, "FrequencyTime");
			Scribe_Collections.Look(ref _values.FrequencyAmount, "FrequencyAmount");
			Scribe_Collections.Look(ref _values.FrequencyChanceFactor, "FrequencyChanceFactor");
			Scribe_Collections.Look(ref _values.SilverScaling, "SilverScaling");
			Scribe_Collections.Look(ref _values.StockScaling, "StockScaling");
			Scribe_Collections.Look(ref _values.WealthScaling, "WealthScaling");
		}
	}
}