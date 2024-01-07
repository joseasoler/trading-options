using HarmonyLib;
using RimWorld;
using TO.Mod;
using Verse;

namespace TradingOptions.Patches.ExtraCaravanVisitorFood;

[HarmonyPatch(typeof(PawnInventoryGenerator), "GiveRandomFood")]
internal static class PawnInventoryGenerator_GiveRandomFood_Patch
{
	private static void Postfix(Pawn p)
	{
		if (Helper.CategoryBeingGenerated == TraderKindCategory.None)
		{
			return;
		}

		// Assume that the pawn already has enough food for a day.
		var extraTicks = Settings.GetDepartureTime(Helper.CategoryBeingGenerated) - GenDate.TicksPerDay;
		var nutritionPerTick = p.needs?.food?.FoodFallPerTick ?? 0.0f;
		var extraNutrition = extraTicks * nutritionPerTick;
		if (extraNutrition <= 0.0f)
		{
			return;
		}

		// Unless the pawn has specific preferences, fine meals are used to try to avoid mental breaks.
		var foodDef = p.kindDef.invFoodDef ?? ThingDefOf.MealFine;
		var food = ThingMaker.MakeThing(foodDef);
		food.stackCount = GenMath.RoundRandom(extraNutrition / food.GetStatValue(StatDefOf.Nutrition));
		p.inventory.TryAddItemNotForSale(food);
	}
}