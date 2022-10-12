using HarmonyLib;
using RimWorld;
using TO.Mod;

namespace TO.Harmony
{
	[HarmonyPatch]
	public static class TraderKind
	{

		[HarmonyPostfix]
		[HarmonyPatch(typeof(TraderKindDef), nameof(TraderKindDef.CalculatedCommonality), MethodType.Getter)]
		private static void CalculatedCommonalityPostfix(ref TraderKindDef __instance, ref float __result)
		{
			if (Settings.GetSlaverNormalCommonality() && __instance.commonalityMultFromPopulationIntent != null)
			{
				__result = __instance.commonality;
			}
		}
	}
}