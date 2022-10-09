using System.Collections.Generic;
using Verse;

namespace TO.Mod
{
	public class SettingValues
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
		
		/// <summary>
		/// Silver stock of each trader category in %.
		/// </summary>
		public Dictionary<TraderKindCategory, int> SilverScaling = new Dictionary<TraderKindCategory, int>
		{
			{TraderKindCategory.Orbital, 100},
			{TraderKindCategory.Settlement, 100},
			{TraderKindCategory.Caravan, 100},
			{TraderKindCategory.Visitor, 100}
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

		/// <summary>
		/// Minimum allowed value for SilverScaling settings in %.
		/// </summary>
		public const int MinSilverScaling = 5;

		/// <summary>
		/// Maximum allowed value for SilverScaling settings in %.
		/// </summary>
		public const int MaxSilverScaling = 1000;

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
			Scribe_Collections.Look(ref _values.SilverScaling, "SilverScaling");
			Scribe_Collections.Look(ref _values.FrequencyTime, "FrequencyTime");
			Scribe_Collections.Look(ref _values.FrequencyAmount, "FrequencyAmount");
			Scribe_Collections.Look(ref _values.FrequencyChanceFactor, "FrequencyChanceFactor");
		}
	}
}