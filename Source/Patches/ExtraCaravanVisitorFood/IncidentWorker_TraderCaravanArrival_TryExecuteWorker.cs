using HarmonyLib;
using RimWorld;

namespace TradingOptions.Patches.ExtraCaravanVisitorFood
{
	[HarmonyPatch(typeof(IncidentWorker_TraderCaravanArrival), "TryExecuteWorker")]
	public class IncidentWorker_TraderCaravanArrival_TryExecuteWorker
	{
		private static void Prefix()
		{
			Helper.CategoryBeingGenerated = TraderKindCategory.Caravan;
		}

		private static void Postfix()
		{
			Helper.CategoryBeingGenerated = TraderKindCategory.None;
		}
	}
}