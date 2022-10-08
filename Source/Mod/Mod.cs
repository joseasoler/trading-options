using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace TO.Mod
{
	/// <summary>
	/// Loads mod settings and displays the mod settings window.
	/// </summary>
	public class Mod : Verse.Mod
	{
		private TraderKindCategory _selected = TraderKindCategory.None;

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

		private static void DrawSettings(TraderKindCategory category, Rect settingsArea)
		{
			var listing = new Listing_Standard();
			listing.Begin(settingsArea);
			if (category != TraderKindCategory.None)
			{
				var categoryName = Enum.GetName(typeof(TraderKindCategory), category);

				var tooltipRect = listing.Label("TO_SilverStock".Translate((int) Settings.GetSilverScaling(category)));
				var silverScaling = listing.Slider(Settings.GetSilverScaling(category), Settings.MinSilverScaling,
					Settings.MaxSilverScaling);
				Settings.SetSilverScaling(category, silverScaling);

				tooltipRect.height += 22.0f; // Height of an slider.
				TooltipHandler.TipRegion(tooltipRect, $"TO_SilverStock{categoryName}Tooltip".Translate());
			}

			var resetButtonWidth = settingsArea.width / 5.0f;
			const float resetButtonHeight = 30.0f;
			var resetButtonX = settingsArea.width - resetButtonWidth - 10.0f;
			var resetButtonY = settingsArea.height - resetButtonHeight - 10.0f;
			var resetButtonRect = new Rect(resetButtonX, resetButtonY, resetButtonWidth, resetButtonHeight);
			if (Widgets.ButtonText(resetButtonRect, "TO_ResetSettings".Translate()))
			{
				Settings.Reset();
			}

			TooltipHandler.TipRegion(resetButtonRect, "TO_ResetSettingsTooltip".Translate());

			listing.End();
		}

		private List<TabRecord> Tabs()
		{
			var tabs = (from TraderKindCategory category in Enum.GetValues(typeof(TraderKindCategory))
				let categoryName = Enum.GetName(typeof(TraderKindCategory), category)
				select new TabRecord($"TO_Category{categoryName}".Translate(), () =>
					{
						_selected = category;
						WriteSettings();
					},
					category == _selected)).ToList();

			return tabs;
		}

		/// <summary>
		/// Contents of the mod settings window.
		/// </summary>
		/// <param name="inRect">Available area for drawing the settings.</param>
		public override void DoSettingsWindowContents(Rect inRect)
		{
			const float tabHeight = 30.0f;
			var tabArea = new Rect(inRect.x, inRect.y + tabHeight, inRect.width, tabHeight);
			var settingsArea = new Rect(inRect.x, inRect.y + tabHeight, inRect.width, inRect.height - tabHeight);

			Widgets.DrawMenuSection(settingsArea);
			TabDrawer.DrawTabs(tabArea, Tabs());
			DrawSettings(_selected, settingsArea.ContractedBy(15.0f));

			base.DoSettingsWindowContents(inRect);
		}
	}
}