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
			if (def != ThingDefOf.Silver)
			{
				return;
			}

			var category = Category.Get(__instance.trader);
			__result = (int) (Settings.GetSilverScaling(category) * __result / 100.0f);
		}
	}
}