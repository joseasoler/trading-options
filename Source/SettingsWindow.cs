using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using TO.Mod;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace TradingOptions;

public static class SettingsWindow
{
	
	private const float MarginProportion = 0.02f;
	private const float WidthProportion = (1.0f - MarginProportion * 2.0f) / 3.0f;

	private static TraderKindCategory _selected = TraderKindCategory.Caravan;
	private const float MinWealth = 10000f;
	private static float _wealth = MinWealth;
	private static string _wealthBuffer = ((int)MinWealth).ToString();

	/// <summary>
	/// Name of the mod in the settings list.
	/// </summary>
	/// <returns>Name of the mod in the settings list.</returns>
	public static string SettingsCategory()
	{
		return Mod.Name;
	}

	private class TripleSliderData
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
		listing.Gap(5.0f);

		Text.Font = GameFont.Small;
		listing.Label(data.Description);

		var labelsRect = listing.GetRect(22.0f);
		var margin = labelsRect.width * MarginProportion;
		var width = labelsRect.width * WidthProportion;
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
		var leftNew = Widgets.HorizontalSlider_NewTemp(leftRect, data.LeftValue, data.LeftMin, data.LeftMax);
		var centerNew = Widgets.HorizontalSlider_NewTemp(centerRect, data.CenterValue, data.CenterMin, data.CenterMax);
		var rightNew = Widgets.HorizontalSlider_NewTemp(rightRect, data.RightValue, data.RightMin, data.RightMax);

		const double tolerance = 0.000001f;
		if (Math.Abs(leftNew - data.LeftValue) > tolerance || Math.Abs(centerNew - data.CenterValue) > tolerance ||
		    Math.Abs(rightNew - data.RightValue) > tolerance)
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
			LeftMax = cat == TraderKindCategory.Orbital ? 4 : 30,
			CenterValue = time,
			CenterMin = 0.0f,
			CenterMax = 20.0f,
			RightValue = chance,
			RightMin = 0.0f,
			RightMax = 500.0f
		});

		var newAmount = (int)result.Left;
		var newTime = (int)result.Center;
		// For caravans and visitors, minSpacingDays must be small enough to fit all caravans in a year.
		if (cat != TraderKindCategory.Orbital)
		{
			var compareTime = newTime;
			if (compareTime == 0)
			{
				compareTime = cat == TraderKindCategory.Caravan ? 6 : 5;
			}

			if (newAmount + newAmount * compareTime > 60)
			{
				newTime = (60 - newAmount) / newAmount;
			}
		}

		Settings.SetFrequencyAmount(cat, newAmount);
		Settings.SetFrequencyTime(cat, newTime);
		Settings.SetFrequencyChanceFactor(cat, (int)result.Right);
	}

	private static void DrawRestockFrequency(Listing_Standard listing, TraderKindCategory cat, string catName)
	{
		if (cat != TraderKindCategory.Settlement)
		{
			return;
		}

		Text.Font = GameFont.Medium;
		listing.Label("TO_RestockFrequencyTitle".Translate());
		listing.Gap(5.0f);

		var restockTime = Settings.GetFrequencyTime(cat);
		var restockTimeChanged = restockTime > 0;
		var description =
			restockTimeChanged ? "TO_RestockFrequency".Translate(restockTime) : "TO_RestockFrequencyDefault".Translate();

		Text.Font = GameFont.Small;
		listing.Label(description);

		var labelRect = listing.GetRect(22.0f);
		Text.Anchor = TextAnchor.MiddleCenter;
		Text.Font = GameFont.Tiny;
		GUI.color = Color.grey;
		Widgets.Label(labelRect, "TO_RestockFrequencySlider".Translate());
		GUI.color = Color.white;
		Text.Anchor = TextAnchor.UpperLeft;
		Text.Font = GameFont.Small;

		var slidersRect = listing.GetRect(22.0f);
		var newRestockTime =
			(int)Widgets.HorizontalSlider_NewTemp(slidersRect, restockTime, SettingValues.DefaultFrequencyTime, 60);
		Settings.SetFrequencyTime(cat, newRestockTime);
	}

	private static void DrawDepartureTime(Listing_Standard listing, TraderKindCategory cat, string catName)
	{
		if (cat == TraderKindCategory.Settlement || cat == TraderKindCategory.None)
		{
			return;
		}

		Text.Font = GameFont.Medium;
		listing.Label("TO_DepartureTitle".Translate());
		listing.Gap(5.0f);

		var departureTime = Settings.GetDepartureTime(cat);
		var departureTimeChanged = departureTime > 0;
		var description =
			departureTimeChanged
				? $"TO_{catName}Departure".Translate(departureTime.ToStringTicksToPeriodVerbose())
				: $"TO_{catName}DepartureDefault".Translate();

		Text.Font = GameFont.Small;
		listing.Label(description);

		var labelRect = listing.GetRect(22.0f);
		Text.Anchor = TextAnchor.MiddleCenter;
		Text.Font = GameFont.Tiny;
		GUI.color = Color.grey;
		Widgets.Label(labelRect, "TO_DepartureSlider".Translate());
		GUI.color = Color.white;
		Text.Anchor = TextAnchor.UpperLeft;
		Text.Font = GameFont.Small;

		var slidersRect = listing.GetRect(22.0f);
		var newDepartureTime =
			(int)Widgets.HorizontalSlider_NewTemp(slidersRect, departureTime, SettingValues.DefaultDepartureTime,
				GenDate.TicksPerDay * 3);
		Settings.SetDepartureTime(cat, newDepartureTime);
	}

	private static void DrawStockAdjustments(Listing_Standard listing, TraderKindCategory cat, string catName)
	{
		var silver = Settings.GetSilverScaling(cat);
		var stock = Settings.GetStockScaling(cat);
		var wealth = Settings.GetWealthScalingOption(cat);

		var silverChanged = silver > Settings.MinStockScaling;
		var stockChanged = stock > Settings.MinStockScaling;
		var wealthChanged = wealth != WealthScalingOption.None;
		var anyChanges = silverChanged || stockChanged || wealthChanged;

		var description = $"TO_{catName}StockPrefix".Translate();
		description +=
			' ' + (silverChanged ? "TO_StockSilver".Translate(silver) : "TO_StockSilverDefault".Translate());
		description +=
			' ' + (stockChanged ? "TO_StockStock".Translate(stock) : "TO_StockStockDefault".Translate());
		description +=
			' ' + (wealthChanged ? "TO_StockWealth".Translate() : "TO_StockWealthDefault".Translate());
		description += ' ' + (anyChanges ? "TO_StockChanged".Translate() : "TO_StockUnchanged".Translate());

		var result = TripleSlider(listing, new TripleSliderData
		{
			Title = "TO_StockTitle".Translate(),
			Description = description,
			LeftLabel = "TO_SilverSlider".Translate(),
			CenterLabel = "TO_StockSlider".Translate(),
			RightLabel =
				"TO_WealthSlider".Translate($"TO_Wealth{Enum.GetName(typeof(WealthScalingOption), wealth)}".Translate()),
			LeftValue = silver,
			LeftMin = Settings.MinStockScaling,
			LeftMax = Settings.MaxStockScaling,
			CenterValue = stock,
			CenterMin = Settings.MinStockScaling,
			CenterMax = Settings.MaxStockScaling,
			RightValue = (float)wealth,
			RightMin = (float)WealthScalingOption.None,
			RightMax = (float)WealthScalingOption.Excessive
		});

		Settings.SetSilverScaling(cat, (int)result.Left);
		Settings.SetStockScaling(cat, (int)result.Center);
		Settings.SetWealthScalingOption(cat, (WealthScalingOption)result.Right);
	}

	private static void DrawStockInfo(Listing_Standard listing, TraderKindCategory category)
	{
		Text.Font = GameFont.Medium;
		listing.Label("TO_InfoTitle".Translate());
		listing.Gap(5.0f);

		Text.Font = GameFont.Small;
		listing.Label("TO_InfoDescription".Translate());
		listing.Gap(5.0f);

		var labelsRect = listing.GetRect(Text.LineHeight);
		var margin = labelsRect.width * MarginProportion;
		var width = labelsRect.width * WidthProportion;

		var leftRect = new Rect(labelsRect.x, labelsRect.y, width, labelsRect.height);
		var centerRect = new Rect(leftRect.x + width + margin, leftRect.y, width, leftRect.height);
		var rightRect = new Rect(centerRect.x + width + margin, centerRect.y, width, centerRect.height);

		var anchor = Text.Anchor;
		Text.Anchor = TextAnchor.MiddleCenter;
		Widgets.TextFieldNumeric(leftRect, ref _wealth, ref _wealthBuffer, MinWealth, float.MaxValue);

		var silver = (int)(100.0 * StockScaling.Calculate(category, ThingDefOf.Silver, _wealth));
		Widgets.Label(centerRect, "TO_InfoSilver".Translate(silver));
		var stock = (int)(100.0 * StockScaling.Calculate(category, null, _wealth));
		Widgets.Label(rightRect, "TO_InfoStock".Translate(stock));
		Text.Anchor = anchor;
	}

	private static void DrawSettings(TraderKindCategory category, Rect settingsArea)
	{
		var listing = new Listing_Standard();
		listing.Begin(settingsArea);
		if (category != TraderKindCategory.None)
		{
			var categoryName = Enum.GetName(typeof(TraderKindCategory), category);

			DrawTraderFrequency(listing, category, categoryName);
			DrawRestockFrequency(listing, category, categoryName);
			listing.Gap(7.0f);
			DrawDepartureTime(listing, category, categoryName);
			listing.Gap(7.0f);
			DrawStockAdjustments(listing, category, categoryName);
			if (Settings.GetWealthScalingOption(category) != WealthScalingOption.None)
			{
				listing.Gap(7.0f);
				DrawStockInfo(listing, category);
			}
		}
		else
		{
			var slaverNormalCommonality = Settings.GetSlaverNormalCommonality();
			listing.CheckboxLabeled("TO_SlaverCommonality".Translate(), ref slaverNormalCommonality);
			Settings.SetSlaverNormalCommonality(slaverNormalCommonality);

			var excludeAnimals = Settings.GetExcludeAnimals();
			listing.CheckboxLabeled("TO_ExcludeAnimals".Translate(), ref excludeAnimals);
			Settings.SetExcludeAnimals(excludeAnimals);
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

	private static List<TabRecord> Tabs(Mod mod)
	{
		var tabs = (from TraderKindCategory category in Enum.GetValues(typeof(TraderKindCategory))
			let categoryName = Enum.GetName(typeof(TraderKindCategory), category)
			select new TabRecord($"TO_Category{categoryName}".Translate(), () =>
				{
					_selected = category;
					mod.WriteSettings();
				},
				category == _selected)).ToList();

		return tabs;
	}

	/// <summary>
	/// Contents of the mod settings window.
	/// </summary>
	/// <param name="inRect">Available area for drawing the settings.</param>
	public static void DoWindowContents(Rect inRect, Mod mod)
	{
		const float tabHeight = 30.0f;
		var tabArea = new Rect(inRect.x, inRect.y + tabHeight, inRect.width, tabHeight);
		var settingsArea = new Rect(inRect.x, inRect.y + tabHeight, inRect.width, inRect.height - tabHeight);

		Widgets.DrawMenuSection(settingsArea);
		TabDrawer.DrawTabs(tabArea, Tabs(mod));
		DrawSettings(_selected, settingsArea.ContractedBy(15.0f));

	}
}