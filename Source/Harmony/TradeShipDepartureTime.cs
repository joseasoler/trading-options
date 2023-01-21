using HarmonyLib;
using RimWorld;
using TO.Mod;

namespace TO.Harmony
{
	[HarmonyPatch]
	internal class TradeShipDepartureTime
	{
		[HarmonyPostfix]
		[HarmonyPatch(typeof(TradeShip), nameof(TradeShip.GenerateThings))]
		private static void ChangeTicksUntilDeparture(ref TradeShip __instance)
		{
			var ticksUntilDeparture = Settings.GetDepartureTime(TraderKindCategory.Orbital);
			if (ticksUntilDeparture > 0)
			{
				__instance.ticksUntilDeparture = ticksUntilDeparture;
			}
		}
	}
}