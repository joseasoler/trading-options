using System.Reflection;
using HarmonyLib;
using Verse;

namespace TradingOptions.Patches;

public static class Methods
{
	public static readonly MethodInfo Verse_Rand_Range =
		AccessTools.Method(typeof(Rand), nameof(Rand.Range), new[] { typeof(int), typeof(int) });
}