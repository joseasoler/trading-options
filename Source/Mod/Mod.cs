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

		class TripleSliderData
		{
			public TaggedString Title;
			public TaggedString Description;
			public TaggedString LeftLabel;
			public TaggedString CenterLabel;
			public TaggedString RightLabel;
			public float LeftValue;
			public float LeftMin;
			public float LeftMax;
			public float CenterValue;
			public float CenterMin;
			public float CenterMax;
			public float RightValue;
			public float RightMin;
			public float RightMax;
		}

		class TripleSliderResult
		{
			public float Left;
			public float Center;
			public float Right;
		}

		private static TripleSliderResult TripleSlider(Listing_Standard listing, TripleSliderData data)
		{
			Text.Font = GameFont.Medium;
			listing.Label(data.Title);
			listing.Gap(6f);

			Text.Font = GameFont.Small;
			listing.Label(data.Description);

			const float marginProportion = 0.02f;
			const float widthProportion = (1.0f - marginProportion * 2.0f) / 3.0f;

			var labelsRect = listing.GetRect(22.0f);
			var margin = labelsRect.width * marginProportion;
			var width = labelsRect.width * widthProportion;
			var leftRect = new Rect(labelsRect.x, labelsRect.y, width, labelsRect.height);
			var centerRect = new Rect(leftRect.x + width + margin, leftRect.y, width, leftRect.height);
			var rightRect = new Rect(centerRect.x + width + margin, centerRect.y, width, centerRect.height);
			Text.Anchor = TextAnchor.MiddleCenter;
			Text.Font = GameFont.Tiny;
			GUI.color = Color.grey;
			Widgets.Label(leftRect, data.LeftLabel);
			Widgets.Label(centerRect, data.CenterLabel);
			Widgets.Label(rightRect, data.RightLabel);
			GUI.color = Color.white;
			Text.Anchor = TextAnchor.UpperLeft;
			Text.Font = GameFont.Small;

			var slidersRect = listing.GetRect(22.0f);
			leftRect.y += slidersRect.height;
			centerRect.y += slidersRect.height;
			rightRect.y += slidersRect.height;
			var leftNew = Widgets.HorizontalSlider(leftRect, data.LeftValue, data.LeftMin, data.LeftMax);
			var centerNew = Widgets.HorizontalSlider(centerRect, data.CenterValue, data.CenterMin, data.CenterMax);
			var rightNew = Widgets.HorizontalSlider(rightRect, data.RightValue, data.RightMin, data.RightMax);

			if (leftNew != data.LeftValue || centerNew != data.CenterValue || rightNew != data.RightValue)
			{
				SoundDefOf.DragSlider.PlayOneShotOnCamera();
			}

			return new TripleSliderResult
			{
				Left = leftNew,
				Center = centerNew,
				Right = rightNew
			};
		}

		private static void DrawTraderFrequency(Listing_Standard listing, TraderKindCategory cat, string catName)
		{
			if (cat == TraderKindCategory.Settlement || cat == TraderKindCategory.None)
			{
				return;
			}

			var amount = Settings.GetFrequencyAmount(cat);
			var time = Settings.GetFrequencyTime(cat);
			var chance = Settings.GetFrequencyChanceFactor(cat);

			var amountChanged = amount > 0;
			var timeChanged = time > 0;
			var chanceChanged = chance > 0;

			var description =
				amountChanged ? $"TO_{catName}Amount".Translate(amount) : $"TO_{catName}AmountDefault".Translate();
			description +=
				' ' + (timeChanged ? $"TO_{catName}Time".Translate(time) : $"TO_{catName}TimeDefault".Translate());
			description += ' ' +
			               (chanceChanged
				               ? $"TO_{catName}Chance".Translate(chance)
				               : $"TO_{catName}ChanceDefault".Translate());
			description += ' ' + (amountChanged || timeChanged || chanceChanged
				? "TO_FrequencyChanged".Translate()
				: "TO_FrequencyUnchanged".Translate());

			var result = TripleSlider(listing, new TripleSliderData
			{
				Title = "TO_FrequencyTitle".Translate(),
				Description = description,
				LeftLabel = $"TO_{catName}AmountSlider".Translate(),
				CenterLabel = $"TO_{catName}TimeSlider".Translate(),
				RightLabel = "TO_ChanceSlider".Translate(),
				LeftValue = amount,
				LeftMin = 0.0f,
				LeftMax = cat == TraderKindCategory.Orbital ? 4 : 15,
				CenterValue = time,
				CenterMin = 0.0f,
				CenterMax = 20.0f,
				RightValue = chance,
				RightMin = 0.0f,
				RightMax = 500.0f
			});

			Settings.SetFrequencyAmount(cat, (int) result.Left);
			Settings.SetFrequencyTime(cat, (int) result.Center);
			Settings.SetFrequencyChanceFactor(cat, (int) result.Right);
		}

		private static void DrawStockAdjustments(Listing_Standard listing, TraderKindCategory cat, string catName)
		{
			var silver = Settings.GetSilverScaling(cat);
			var stock = Settings.GetStockScaling(cat);
			var wealth = Settings.GetWealthScalingOption(cat);

			var silverChanged = silver > 0;
			var stockChanged = stock > 0;
			var wealthChanged = wealth != WealthScalingOption.None;

			var description = $"TO_{catName}StockPrefix".Translate();
			description +=
				' ' + (silverChanged ? "TO_StockSilver".Translate(silver) : "TO_StockSilverDefault".Translate());
			description +=
				' ' + (stockChanged ? "TO_StockStock".Translate(stock) : "TO_StockStockDefault".Translate());
			description +=
				' ' + (wealthChanged ? "TO_StockWealth".Translate() : "TO_StockWealthDefault".Translate());
			description += ' ' + (silverChanged || stockChanged || wealthChanged
				? "TO_StockChanged".Translate()
				: "TO_StockUnchanged".Translate());

			var result = TripleSlider(listing, new TripleSliderData
			{
				Title = "TO_StockTitle".Translate(),
				Description = description,
				LeftLabel = "TO_SilverSlider".Translate(),
				CenterLabel = "TO_SilverSlider".Translate(),
				RightLabel =
					"TO_WealthSlider".Translate($"TO_Wealth{Enum.GetName(typeof(WealthScalingOption), wealth)}".Translate()),
				LeftValue = silver,
				LeftMin = Settings.MinStockScaling,
				LeftMax = Settings.MaxStockScaling,
				CenterValue = stock,
				CenterMin = Settings.MinStockScaling,
				CenterMax = Settings.MaxStockScaling,
				RightValue = (float) wealth,
				RightMin = (float) WealthScalingOption.None,
				RightMax = (float) WealthScalingOption.Loaded
			});

			Settings.SetSilverScaling(cat, (int) result.Left);
			Settings.SetStockScaling(cat, (int) result.Center);
			Settings.SetWealthScalingOption(cat, (WealthScalingOption) result.Right);
		}

		private static void DrawWealthAdjustments(Listing_Standard listing, TraderKindCategory category,
			string categoryName)
		{
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