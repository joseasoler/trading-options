using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using TO.Mod;
using Verse;

namespace TradingOptions.Harmony
{
	[HarmonyPatch]
	internal class CaravanVisitorDepartureTime
	{
		private static readonly MethodInfo RandRange =
			AccessTools.Method(typeof(Rand), nameof(Rand.Range), new[] {typeof(int), typeof(int)});

		private static int GetCaravanDepartureTime(int min, int max)
		{
			var departureTime = Settings.GetDepartureTime(TraderKindCategory.Caravan);
			return departureTime == 0 ? Rand.Range(min, max) : departureTime;
		}

		[HarmonyTranspiler]
		[HarmonyPatch(typeof(LordJob_TradeWithColony), nameof(LordJob_TradeWithColony.CreateGraph))]
		private static IEnumerable<CodeInstruction> PatchCaravanDepartureTime(IEnumerable<CodeInstruction> instructions)
		{
			foreach (var code in instructions)
			{
				if (code.Calls(RandRange))
				{
					yield return CodeInstruction.Call(typeof(CaravanVisitorDepartureTime), nameof(GetCaravanDepartureTime));
				}
				else
				{
					yield return code;
				}
			}
		}

		private static int GetVisitorDepartureTime(int min, int max)
		{
			var departureTime = Settings.GetDepartureTime(TraderKindCategory.Visitor);
			return departureTime == 0 ? Rand.Range(min, max) : departureTime;
		}

		[HarmonyTranspiler]
		[HarmonyPatch(typeof(LordJob_VisitColony), nameof(LordJob_VisitColony.CreateGraph))]
		private static IEnumerable<CodeInstruction> PatchVisitorDepartureTime(IEnumerable<CodeInstruction> instructions)
		{
			foreach (var code in instructions)
			{
				if (code.Calls(RandRange))
				{
					yield return CodeInstruction.Call(typeof(CaravanVisitorDepartureTime), nameof(GetVisitorDepartureTime));
				}
				else
				{
					yield return code;
				}
			}
		}
	}
}