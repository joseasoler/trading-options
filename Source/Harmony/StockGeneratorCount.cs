using HarmonyLib;
using RimWorld;
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
			var scaling = StockScaling.Calculate(category, def, Find.World.PlayerWealthForStoryteller);
			__result = (int) (scaling * __result);
		}
	}
}