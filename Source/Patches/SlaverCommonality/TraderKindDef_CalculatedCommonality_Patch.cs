using HarmonyLib;
using RimWorld;
using TO.Mod;

namespace TradingOptions.Patches
{
	[HarmonyPatch(typeof(TraderKindDef), nameof(TraderKindDef.CalculatedCommonality), MethodType.Getter)]
	public static class TraderKindDef_CalculatedCommonality_Patch
	{
		private static void Postfix(ref TraderKindDef __instance, ref float __result)
		{
			if (Settings.GetSlaverNormalCommonality() && __instance.commonalityMultFromPopulationIntent != null)
			{
				__result = __instance.commonality;
			}
		}
	}
}