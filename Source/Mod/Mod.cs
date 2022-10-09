using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using RimWorld;
using UnityEngine;
using UnityEngine.Assertions;
using Verse;
using Verse.Sound;

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

		public override void WriteSettings()
		{
			base.WriteSettings();
			// ToDo
			// DefPatcher.Patch();
		}

		private static int LabelAndSlider(Listing_Standard listing, string label, int value, int minValue,
			int maxValue, string tooltip)
		{
			var tooltipRect = listing.Label(label);
			var result = listing.Slider(value, minValue, maxValue);

			tooltipRect.width = listing.ColumnWidth;
			tooltipRect.height += 22.0f; // Height of an slider.
			TooltipHandler.TipRegion(tooltipRect, tooltip);
			return (int) result;
		}

		private static void DrawTraderFrequency(Listing_Standard listing, TraderKindCategory category, string categoryName)
		{
			if (category == TraderKindCategory.Settlement || category == TraderKindCategory.None)
			{
				var logError = $"[TO] Invalid category {categoryName} in DrawTraderFrequency";
				Log.ErrorOnce(logError, logError.GetHashCode());
				return;
			}

			Text.Font = GameFont.Medium;
			listing.Label("TO_FrequencyTitle".Translate());
			listing.Gap();

			Text.Font = GameFont.Small;
			var amount = Settings.GetFrequencyAmount(category);
			string amountLabel = amount > 0
				? $"TO_{categoryName}FrequencyAmount".Translate(amount)
				: $"TO_{categoryName}FrequencyAmountDefault".Translate();

			var time = Settings.GetFrequencyTime(category);
			string timeLabel = time > 0
				? $"TO_{categoryName}FrequencyTime".Translate(time)
				: $"TO_{categoryName}FrequencyTimeDefault".Translate();

			var nonRandomPostfix = amount > 0 || time > 0 ? "TO_MinimumIsUnchanged" : "TO_HowToAdjust";
			var nonRandomLabel = "TO_NonRandom".Translate(amountLabel, timeLabel,
				$"TO_{categoryName}FrequencyVanilla".Translate(),
				nonRandomPostfix.Translate());

			listing.Label(nonRandomLabel);
			var sliderLabelsRect = listing.GetRect(22.0f);
			const float splitPart = 0.48f;
			Text.Anchor = TextAnchor.MiddleCenter;
			Text.Font = GameFont.Tiny;
			GUI.color = Color.grey;
			Widgets.Label(sliderLabelsRect.LeftPart(splitPart), $"TO_{categoryName}FrequencyAmountLabel".Translate());
			Widgets.Label(sliderLabelsRect.RightPart(splitPart), $"TO_{categoryName}FrequencyTimeLabel".Translate());
			GUI.color = Color.white;
			Text.Anchor = TextAnchor.UpperLeft;
			Text.Font = GameFont.Small;
			var slidersRect = listing.GetRect(22.0f);
			const int maxAmountOrbital = 4;
			const int maxAmountOthers = 15;
			var maxAmount = category == TraderKindCategory.Orbital ? maxAmountOrbital : maxAmountOthers;
			var newAmount = (int) Widgets.HorizontalSlider(slidersRect.LeftPart(splitPart), amount, 0, maxAmount);
			var newTime = (int) Widgets.HorizontalSlider(slidersRect.RightPart(splitPart), time, 0, 20);
			if (newAmount != amount || newTime != time)
			{
				SoundDefOf.DragSlider.PlayOneShotOnCamera();
			}

			Settings.SetFrequencyAmount(category, newAmount);
			Settings.SetFrequencyTime(category, newTime);
			listing.Gap();

			var chance = Settings.GetFrequencyChanceFactor(category);
			string chanceLabel = chance > 0
				? $"TO_{categoryName}FrequencyChanceFactor".Translate(chance, "TO_MinimumIsUnchanged".Translate())
				: $"TO_{categoryName}FrequencyChanceFactorDefault".Translate("TO_HowToAdjust".Translate());
			listing.Label(chanceLabel);
			var newChance = (int) listing.Slider(chance, 0, 500);
			Settings.SetFrequencyChanceFactor(category, newChance);
		}

		private static void DrawSilverStockAdjustment(Listing_Standard listing, TraderKindCategory category,
			string categoryName)
		{
			var prevValue = Settings.GetSilverScaling(category);
			var label = "TO_SilverStock".Translate(prevValue);
			var tooltip = $"TO_SilverStock{categoryName}Tooltip".Translate();
			var value = LabelAndSlider(listing, label, prevValue, Settings.MinSilverScaling,
				Settings.MaxSilverScaling, tooltip);
			Settings.SetSilverScaling(category, value);
		}

		private static void DrawSettings(TraderKindCategory category, Rect settingsArea)
		{
			var listing = new Listing_Standard();
			listing.Begin(settingsArea);
			if (category != TraderKindCategory.None)
			{
				var categoryName = Enum.GetName(typeof(TraderKindCategory), category);

				DrawTraderFrequency(listing, category, categoryName);
				listing.GapLine(listing.verticalSpacing);
				listing.Gap();
				DrawSilverStockAdjustment(listing, category, categoryName);
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