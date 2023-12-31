using System;
using System.Collections.Generic;
using TradingOptions;
using Verse;

// Old namespace kept for config file compatibility.
namespace TO.Mod
{
	internal class SettingValues
	{
		public const int DefaultFrequencyTime = 0;

		/// <summary>
		/// Time value for changing trader frequency. The meaning of this value depends on each trader.
		/// Zero always means "let the storyteller decide".
		/// </summary>
		public Dictionary<TraderKindCategory, int> FrequencyTime = new()
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
		public Dictionary<TraderKindCategory, int> FrequencyAmount = new()
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
		public Dictionary<TraderKindCategory, int> FrequencyChanceFactor = new()
		{
			{TraderKindCategory.Orbital, DefaultFrequencyChanceFactor},
			{TraderKindCategory.Caravan, DefaultFrequencyChanceFactor}
		};

		public static readonly int DefaultDepartureTime = 0;

		/// <summary>
		/// Number of ticks that the trader will stay available.
		/// Zero always means "let the storyteller decide".
		/// </summary>
		public Dictionary<TraderKindCategory, int> DepartureTime = new()
		{
			{TraderKindCategory.Orbital, DefaultDepartureTime},
			{TraderKindCategory.Caravan, DefaultDepartureTime},
			{TraderKindCategory.Visitor, DefaultDepartureTime}
		};

		public const int MinScaling = 24;

		/// <summary>
		/// Silver stock of each trader category in %.
		/// </summary>
		public Dictionary<TraderKindCategory, int> SilverScaling = new()
		{
			{TraderKindCategory.Orbital, MinScaling},
			{TraderKindCategory.Settlement, MinScaling},
			{TraderKindCategory.Caravan, MinScaling},
			{TraderKindCategory.Visitor, MinScaling}
		};

		/// <summary>
		/// Other stock of each trader category in %.
		/// </summary>
		public Dictionary<TraderKindCategory, int> StockScaling = new()
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
			new()
			{
				{TraderKindCategory.Orbital, WealthScalingOption.None},
				{TraderKindCategory.Settlement, WealthScalingOption.None},
				{TraderKindCategory.Caravan, WealthScalingOption.None},
				{TraderKindCategory.Visitor, WealthScalingOption.None}
			};

		/// <summary>
		/// Slaver traders will appear regardless of colony population.
		/// </summary>
		public bool SlaverNormalCommonality /* = false */;

		/// <summary>
		/// Exclude animals from stock increases. This may help with performance issues when traders are present.
		/// </summary>
		public bool ExcludeAnimals /* = false */;
	}

	/// <summary>
	/// Handles mod settings.
	/// </summary>
	public class Settings : ModSettings
	{
		/// <summary>
		/// Current values for all settings.
		/// </summary>
		private static SettingValues _values = new();

		public static int GetFrequencyTime(TraderKindCategory category)
		{
			// ToDo clean up compatibility code: addition of restock frequency for settlements.
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

		public static int GetDepartureTime(TraderKindCategory category)
		{
			// ToDo clean up compatibility code: addition of time until departure.
			if (_values.DepartureTime.NullOrEmpty())
			{
				var tempSettings = new SettingValues();
				_values.DepartureTime = tempSettings.DepartureTime;
			}

			return _values.DepartureTime[category];
		}

		public static void SetDepartureTime(TraderKindCategory category, int value)
		{
			_values.DepartureTime[category] = value;
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
		public const int MaxStockScaling = 800;

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

		public static void ResetCategory(TraderKindCategory category)
		{
			SettingValues resetValues = new SettingValues();

			_values.FrequencyTime[category] = resetValues.FrequencyTime[category];
			_values.SilverScaling[category] = resetValues.SilverScaling[category];
			_values.StockScaling[category] = resetValues.StockScaling[category];
			_values.WealthScaling[category] = resetValues.WealthScaling[category];

			if (category != TraderKindCategory.Settlement)
			{
				_values.FrequencyAmount[category] = resetValues.FrequencyAmount[category];
				_values.DepartureTime[category] = resetValues.DepartureTime[category];
			}

			if (category == TraderKindCategory.Orbital || category == TraderKindCategory.Caravan)
			{
				_values.FrequencyChanceFactor[category] = resetValues.FrequencyChanceFactor[category];
			}
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
			Scribe_Collections.Look(ref _values.DepartureTime, "DepartureTime");
			Scribe_Collections.Look(ref _values.SilverScaling, "SilverScaling");
			Scribe_Collections.Look(ref _values.StockScaling, "StockScaling");
			Scribe_Collections.Look(ref _values.WealthScaling, "WealthScaling");
			Scribe_Values.Look(ref _values.SlaverNormalCommonality, "SlaverNormalCommonality");
			Scribe_Values.Look(ref _values.ExcludeAnimals, "ExcludeAnimals");
		}
	}
}