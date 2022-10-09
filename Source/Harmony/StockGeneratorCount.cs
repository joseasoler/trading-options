using HarmonyLib;
using RimWorld;
using TO.Mod;
using Verse;

namespace TO.Harmony
{
	[HarmonyPatch]
	public static class StockGeneratorCount
	{


		[HarmonyPostfix]
		[HarmonyPatch(typeof(StockGenerator), "RandomCountOf")]
		private static void ModifyGeneratedAmounts(ref StockGenerator __instance, ref int __result, ThingDef def)
		{
			var category = Category.Get(__instance.trader);
			var scaling = def == ThingDefOf.Silver ? Settings.GetSilverScaling(category) : Settings.GetStockScaling(category);
			__result = (int) (scaling * __result / 100.0f);
		}
	}
}