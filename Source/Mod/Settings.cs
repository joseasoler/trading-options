using System;
using System.Collections.Generic;
using Verse;

namespace TO.Mod
{
	internal class SettingValues
	{
		public const int DefaultFrequencyTime = 0;

		/// <summary>
		/// Time value for changing trader frequency. The meaning of this value depends on each trader.
		/// Zero always means "let the storyteller decide".
		/// </summary>
		public Dictionary<TraderKindCategory, int> FrequencyTime = new Dictionary<TraderKindCategory, int>
		{
			{TraderKindCategory.Orbital, DefaultFrequencyTime},
			{TraderKindCategory.Caravan, DefaultFrequencyTime},
			{TraderKindCategory.Settlement, DefaultFrequencyTime},
			{TraderKindCategory.Visitor, DefaultFrequencyTime}
		};

		public const int DefaultFrequencyAmount = 0;

		/// <summary>
		/// Amount value for changing trader frequency. The meaning of this value depends on each trader.
		/// Zero always means "let the storyteller decide".
		/// </summary>
		public Dictionary<TraderKindCategory, int> FrequencyAmount = new Dictionary<TraderKindCategory, int>
		{
			{TraderKindCategory.Orbital, DefaultFrequencyAmount},
			{TraderKindCategory.Caravan, DefaultFrequencyAmount},
			{TraderKindCategory.Visitor, DefaultFrequencyAmount}
		};

		public const int DefaultFrequencyChanceFactor = 100;

		/// <summary>
		/// % factor applied to trader frequency chance when using random storytellers. 
		/// Zero always means "let the storyteller decide".
		/// </summary>
		public Dictionary<TraderKindCategory, int> FrequencyChanceFactor = new Dictionary<TraderKindCategory, int>
		{
			{TraderKindCategory.Orbital, DefaultFrequencyChanceFactor},
			{TraderKindCategory.Caravan, DefaultFrequencyChanceFactor}
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
		public Dictionary<TraderKindCategory, WealthScalingOption> WealthScaling =
			new Dictionary<TraderKindCategory, WealthScalingOption>
			{
				{TraderKindCategory.Orbital, WealthScalingOption.None},
				{TraderKindCategory.Settlement, WealthScalingOption.None},
				{TraderKindCategory.Caravan, WealthScalingOption.None},
				{TraderKindCategory.Visitor, WealthScalingOption.None}
			};

		/// <summary>
		/// Slaver traders will appear regardless of colony population.
		/// </summary>
		public bool SlaverNormalCommonality/* = false */;

		/// <summary>
		/// Exclude animals from stock increases. This may help with performance issues when traders are present.
		/// </summary>
		public bool ExcludeAnimals/* = false */;
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
			// Compatibility code: addition of restock frequency for settlements.
			if (category == TraderKindCategory.Settlement && !_values.FrequencyTime.ContainsKey(category))
			{
				return SettingValues.DefaultFrequencyTime;
			}

			return category == TraderKindCategory.None ? SettingValues.DefaultFrequencyTime : _values.FrequencyTime[category];
		}

		public static void SetFrequencyTime(TraderKindCategory category, int value)
		{
			_values.FrequencyTime[category] = value;
		}

		public static int GetFrequencyAmount(TraderKindCategory category)
		{
			return category == TraderKindCategory.None
				? SettingValues.DefaultFrequencyAmount
				: _values.FrequencyAmount[category];
		}

		public static void SetFrequencyAmount(TraderKindCategory category, int value)
		{
			_values.FrequencyAmount[category] = value;
		}

		public static int GetFrequencyChanceFactor(TraderKindCategory category)
		{
			switch (category)
			{
				case TraderKindCategory.None:
					return SettingValues.DefaultFrequencyChanceFactor;
				case TraderKindCategory.Visitor:
					return _values.FrequencyChanceFactor[TraderKindCategory.Caravan];
				case TraderKindCategory.Caravan:
				case TraderKindCategory.Orbital:
				case TraderKindCategory.Settlement:
				default:
					return _values.FrequencyChanceFactor[category];
			}
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
			return category == TraderKindCategory.None ? 100 : _values.SilverScaling[category];
		}

		public static void SetSilverScaling(TraderKindCategory category, int value)
		{
			_values.SilverScaling[category] = value;
		}

		public static int GetStockScaling(TraderKindCategory category)
		{
			return category == TraderKindCategory.None ? 100 : _values.StockScaling[category];
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

		public static WealthScalingOption GetWealthScalingOption(TraderKindCategory category)
		{
			return category == TraderKindCategory.None ? WealthScalingOption.None : _values.WealthScaling[category];
		}

		public static void SetWealthScalingOption(TraderKindCategory category, WealthScalingOption value)
		{
			_values.WealthScaling[category] = value;
		}

		public static double GetWealthScaling(TraderKindCategory category)
		{
			if (category == TraderKindCategory.None)
			{
				return 0.0;
			}

			switch (_values.WealthScaling[category])
			{
				case WealthScalingOption.None:
					return 0.0;
				case WealthScalingOption.Poor:
					return 1000.0;
				case WealthScalingOption.Rich:
					return 100.0;
				case WealthScalingOption.Opulent:
					return 10.0;
				case WealthScalingOption.Loaded:
					return Math.E;
				case WealthScalingOption.Excessive:
					return Math.E / 2.0f;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public static bool GetSlaverNormalCommonality()
		{
			return _values.SlaverNormalCommonality;
		}

		public static void SetSlaverNormalCommonality(bool value)
		{
			_values.SlaverNormalCommonality = value;
		}

		public static bool GetExcludeAnimals()
		{
			return _values.ExcludeAnimals;
		}

		public static void SetExcludeAnimals(bool value)
		{
			_values.ExcludeAnimals = value;
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
			Scribe_Values.Look(ref _values.SlaverNormalCommonality, "SlaverNormalCommonality");
			Scribe_Values.Look(ref _values.ExcludeAnimals, "ExcludeAnimals");
		}
	}
}