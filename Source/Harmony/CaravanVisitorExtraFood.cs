using HarmonyLib;
using RimWorld;
using TO.Mod;
using Verse;

namespace TradingOptions.Harmony
{
	internal class CaravanVisitorExtraFood
	{
		private static TraderKindCategory CategoryBeingGenerated = TraderKindCategory.None;

		[HarmonyPatch(typeof(PawnInventoryGenerator), "GiveRandomFood")]
		internal class GiveExtraFood
		{
			private static void Postfix(Pawn p)
			{
				if (CategoryBeingGenerated == TraderKindCategory.None)
				{
					return;
				}

				// Assume that the pawn already has enough food for a day.
				var extraTicks = Settings.GetDepartureTime(CategoryBeingGenerated) - GenDate.TicksPerDay;
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

		[HarmonyPatch(typeof(IncidentWorker_TraderCaravanArrival), "TryExecuteWorker")]
		internal class CaravanExtraFood
		{
			private static void Prefix()
			{
				CategoryBeingGenerated = TraderKindCategory.Caravan;
			}

			private static void Postfix()
			{
				CategoryBeingGenerated = TraderKindCategory.None;
			}
		}

		[HarmonyPatch(typeof(IncidentWorker_VisitorGroup), "TryExecuteWorker")]
		internal class VisitorExtraFood
		{
			private static void Prefix()
			{
				CategoryBeingGenerated = TraderKindCategory.Visitor;
			}

			private static void Postfix()
			{
				CategoryBeingGenerated = TraderKindCategory.None;
			}
		}
	}
}