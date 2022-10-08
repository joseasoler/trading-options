using System;
using UnityEngine;
using Verse;

namespace TO.Mod
{
	/// <summary>
	/// Loads mod settings and displays the mod settings window.
	/// </summary>
	public class Mod : Verse.Mod
	{
		/// <summary>
		/// Reads and initializes mod settings.
		/// </summary>
		/// <param name="content">Content pack data of this mod.</param>
		public Mod(ModContentPack content) : base(content)
		{
			GetSettings<Settings>();
		}

		/// <summary>
		/// Name of the mod in the settings list.
		/// </summary>
		/// <returns>Name of the mod in the settings list.</returns>
		public override string SettingsCategory()
		{
			return "Trading Options";
		}

		/// <summary>
		/// Contents of the mod settings window.
		/// </summary>
		/// <param name="inRect">Available area for drawing the settings.</param>
		public override void DoSettingsWindowContents(Rect inRect)
		{
			var listing = new Listing_Standard();
			listing.Begin(inRect);

			foreach (var categoryObj in Enum.GetValues(typeof(TraderKindCategory)))
			{
				var category = (TraderKindCategory) categoryObj;
				if (category == TraderKindCategory.None)
				{
					continue;
				}

				var categoryName = Enum.GetName(typeof(TraderKindCategory), category);

				listing.Label($"TO_SilverStock{categoryName}".Translate((int) Settings.GetSilverScaling(category)), -1,
					$"TO_SilverStock{categoryName}Tooltip".Translate());
				var silverScaling = listing.Slider(Settings.GetSilverScaling(category), Settings.MinSilverScaling,
					Settings.MaxSilverScaling);
				Settings.SetSilverScaling(category, silverScaling);
			}

			listing.Gap();
			var resetButtonRect = listing.GetRect(30f);
			var resetWidth = resetButtonRect.width;
			resetButtonRect.width /= 5f;
			resetButtonRect.x += resetWidth - resetButtonRect.width;
			if (Widgets.ButtonText(resetButtonRect, "TO_ResetSettings".Translate()))
			{
				Settings.Reset();
			}

			TooltipHandler.TipRegion(resetButtonRect, "TO_ResetSettingsTooltip".Translate());


			listing.End();
			base.DoSettingsWindowContents(inRect);
		}
	}
}