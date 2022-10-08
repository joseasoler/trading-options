using System.Collections.Generic;
using Verse;

namespace TO.Mod
{
	public class SettingValues
	{
		/// <summary>
		/// Silver stock of each trader category in %.
		/// </summary>
		public Dictionary<TraderKindCategory, float> SilverScaling = new Dictionary<TraderKindCategory, float>
		{
			{TraderKindCategory.Orbital, 100.0f},
			{TraderKindCategory.Settlement, 100.0f},
			{TraderKindCategory.Caravan, 100.0f},
			{TraderKindCategory.Visitor, 100.0f}
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

		public static float GetSilverScaling(TraderKindCategory category)
		{
			return _values.SilverScaling[category];
		}

		public static void SetSilverScaling(TraderKindCategory category, float value)
		{
			_values.SilverScaling[category] = value;
		}

		/// <summary>
		/// Minimum allowed value for SilverScaling settings in %.
		/// </summary>
		public const float MinSilverScaling = 5.0f;

		/// <summary>
		/// Maximum allowed value for SilverScaling settings in %.
		/// </summary>
		public const float MaxSilverScaling = 1000.0f;

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
		}
	}
}