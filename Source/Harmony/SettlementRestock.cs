using HarmonyLib;
using RimWorld.Planet;
using TO.Mod;
using Verse;

namespace TO.Harmony
{
	[HarmonyPatch]
	internal static class SettlementRestock
	{
		[HarmonyPostfix]
		[HarmonyPatch(typeof(Settlement_TraderTracker), "RegenerateStockEveryDays", MethodType.Getter)]
		private static void ModifySettlementRestockDays(ref int __result)
		{
			__result = Settings.GetFrequencyTime(TraderKindCategory.Settlement);
			if (__result == 0)
			{
				__result = 30;
			}
		}
	}
}