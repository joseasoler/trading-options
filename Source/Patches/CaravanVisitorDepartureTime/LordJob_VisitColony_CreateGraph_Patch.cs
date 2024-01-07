using System.Collections.Generic;
using HarmonyLib;
using RimWorld;

namespace TradingOptions.Patches.CaravanVisitorDepartureTime
{
	/// <summary>
	/// Replace default visitor departure times with a value configured through the mod settings.
	/// </summary>
	[HarmonyPatch(typeof(LordJob_VisitColony), nameof(LordJob_VisitColony.CreateGraph))]
	internal static class LordJob_VisitColony_CreateGraph_Patch
	{
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			return Helper.InjectDepartureTimeIntoRandRange(nameof(LordJob_VisitColony_CreateGraph_Patch), instructions);
		}
	}
}