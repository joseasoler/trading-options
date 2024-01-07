using HarmonyLib;
using RimWorld.Planet;
using TO.Mod;
using TradingOptions;

namespace TradingOptions.Patches.SettlementRestock;

[HarmonyPatch]
internal static class Settlement_TraderTracker_RegenerateStockEveryDays_Patch
{
	[HarmonyPatch(typeof(Settlement_TraderTracker), "RegenerateStockEveryDays", MethodType.Getter)]
	private static void Postfix(ref int __result)
	{
		var setting = Settings.GetFrequencyTime(TraderKindCategory.Settlement);
		if (setting != 0)
		{
			__result = setting;
		}
	}
}