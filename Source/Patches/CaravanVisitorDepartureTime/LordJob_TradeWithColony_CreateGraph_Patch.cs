using System.Collections.Generic;
using HarmonyLib;
using RimWorld;

namespace TradingOptions.Patches.CaravanVisitorDepartureTime
{
	/// <summary>
	/// Replace default caravan departure times with a value configured through the mod settings.
	/// </summary>
	[HarmonyPatch(typeof(LordJob_TradeWithColony), nameof(LordJob_TradeWithColony.CreateGraph))]
	internal static class LordJob_TradeWithColony_CreateGraph_Patch
	{
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			return Helper.InjectDepartureTimeIntoRandRange(nameof(LordJob_TradeWithColony_CreateGraph_Patch), instructions);
		}
	}
}