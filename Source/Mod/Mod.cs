using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace TO.Mod
{
	/// <summary>
	/// Loads mod settings and displays the mod settings window.
	/// </summary>
	public class Mod : Verse.Mod
	{
		private TraderKindCategory _selected = TraderKindCategory.Caravan;

		/// <summary>
		/// Reads and initializes mod settings.
		/// </summary>
		/// <param name="content">Content pack data of this mod.</param>
		public Mod(ModContentPack content) : base(content)
		{
			GetSettings<Settings>();
			LongEventHandler.ExecuteWhenFinished(StorytellerCompPatcher.Patch);
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
			StorytellerCompPatcher.Patch();
		}

		private static void DrawTraderFrequency(Listing_Standard listing, TraderKindCategory category, string categoryName)
		{
			if (category == TraderKindCategory.Settlement || category == TraderKindCategory.None)
			{
				return;
			}

			Text.Font = GameFont.Medium;
			listing.Label("TO_FrequencyTitle".Translate());
			listing.Gap(6f);

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
			listing.Gap(6f);

			var chance = Settings.GetFrequencyChanceFactor(category);
			string chanceLabel = chance > 0
				? $"TO_{categoryName}FrequencyChanceFactor".Translate(chance, "TO_MinimumIsUnchanged".Translate())
				: $"TO_{categoryName}FrequencyChanceFactorDefault".Translate("TO_HowToAdjust".Translate());
			listing.Label(chanceLabel);
			var newChance = (int) listing.Slider(chance, 0, 500);
			Settings.SetFrequencyChanceFactor(category, newChance);
		}

		private static void DrawStockAdjustments(Listing_Standard listing, TraderKindCategory category,
			string categoryName)
		{
			Text.Font = GameFont.Medium;
			listing.Label("TO_StockTitle".Translate());
			listing.Gap(6f);
			Text.Font = GameFont.Small;

			var silverScaling = Settings.GetSilverScaling(category);
			var stockScaling = Settings.GetStockScaling(category);

			var prefixLabel = $"TO_{categoryName}StockPrefix".Translate();
			var silverLabel = silverScaling > Settings.MinStockScaling
				? "TO_StockSilverLabel".Translate(silverScaling)
				: "TO_StockSilverLabelDefault".Translate();
			var stockLabel = stockScaling > Settings.MinStockScaling
				? "TO_StockStockLabel".Translate(stockScaling)
				: "TO_StockStockLabelDefault".Translate();
			var postfixLabel = (silverScaling > Settings.MinStockScaling || stockScaling > Settings.MinStockScaling)
				? "TO_StockMinimumIsUnchanged".Translate()
				: "TO_StockHowToAdjust".Translate();

			var label = "TO_StockLabel".Translate(prefixLabel, silverLabel, stockLabel, postfixLabel);
			listing.Label(label);

			var sliderLabelsRect = listing.GetRect(22.0f);
			const float splitPart = 0.48f;
			Text.Anchor = TextAnchor.MiddleCenter;
			Text.Font = GameFont.Tiny;
			GUI.color = Color.grey;
			Widgets.Label(sliderLabelsRect.LeftPart(splitPart), "TO_StockSilverSlider".Translate());
			Widgets.Label(sliderLabelsRect.RightPart(splitPart), "TO_StockStockSlider".Translate());
			GUI.color = Color.white;
			Text.Anchor = TextAnchor.UpperLeft;
			Text.Font = GameFont.Small;
			var slidersRect = listing.GetRect(22.0f);
			var newSilverScaling = (int) Widgets.HorizontalSlider(slidersRect.LeftPart(splitPart), silverScaling,
				Settings.MinStockScaling, Settings.MaxStockScaling);
			var newStockScaling = (int) Widgets.HorizontalSlider(slidersRect.RightPart(splitPart), stockScaling,
				Settings.MinStockScaling, Settings.MaxStockScaling);
			if (newSilverScaling != silverScaling || newStockScaling != stockScaling)
			{
				SoundDefOf.DragSlider.PlayOneShotOnCamera();
			}

			Settings.SetSilverScaling(category, newSilverScaling);
			Settings.SetStockScaling(category, newStockScaling);
		}

		private static void DrawWealthAdjustments(Listing_Standard listing, TraderKindCategory category,
			string categoryName)
		{
			Text.Font = GameFont.Medium;
			listing.Label("TO_WealthTitle".Translate());
			listing.Gap(6f);
			Text.Font = GameFont.Small;

			var wealthScalingSetting = Settings.GetWealthScalingOption(category);
			var isDefault = wealthScalingSetting == WealthScalingOption.None;
			var prefixLabel = $"TO_{categoryName}StockPrefix".Translate();
			var descriptionLabel = isDefault
				? "TO_WealthDescriptionDefault".Translate()
				: "TO_WealthDescription".Translate();

			var label = "TO_WealthLabel".Translate(prefixLabel, descriptionLabel);
			listing.Label(label);

			Text.Anchor = TextAnchor.MiddleCenter;
			Text.Font = GameFont.Tiny;
			GUI.color = Color.grey;
			listing.Label("TO_WealthSlider".Translate(
				$"TO_WealthValue{Enum.GetName(typeof(WealthScalingOption), wealthScalingSetting)}".Translate()));
			GUI.color = Color.white;
			Text.Anchor = TextAnchor.UpperLeft;
			Text.Font = GameFont.Small;

			var result = listing.Slider((int) wealthScalingSetting, (int) WealthScalingOption.None,
				(int) WealthScalingOption.Loaded);
			// Log.Error($"Logging: {result}, {Enum.GetName(typeof(WealthScalingOption), (WealthScalingOption) result)}");
			Settings.SetWealthScalingOption(category, (WealthScalingOption) result);
		}

		private static void DrawSettings(TraderKindCategory category, Rect settingsArea)
		{
			var listing = new Listing_Standard();
			listing.Begin(settingsArea);
			if (category != TraderKindCategory.None)
			{
				var categoryName = Enum.GetName(typeof(TraderKindCategory), category);

				DrawTraderFrequency(listing, category, categoryName);
				listing.Gap(9f);
				DrawStockAdjustments(listing, category, categoryName);
				listing.Gap(9f);
				DrawWealthAdjustments(listing, category, categoryName);
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