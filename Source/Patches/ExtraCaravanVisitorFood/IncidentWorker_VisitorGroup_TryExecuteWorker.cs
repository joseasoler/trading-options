using HarmonyLib;
using RimWorld;

namespace TradingOptions.Patches.ExtraCaravanVisitorFood;

[HarmonyPatch(typeof(IncidentWorker_VisitorGroup), "TryExecuteWorker")]
public class IncidentWorker_VisitorGroup_TryExecuteWorker
{
	private static void Prefix()
	{
		Helper.CategoryBeingGenerated = TraderKindCategory.Visitor;
	}

	private static void Postfix()
	{
		Helper.CategoryBeingGenerated = TraderKindCategory.None;
	}
}