using System;

namespace TradingOptions;

/// <summary>
/// Initialization of the Harmony patching of the mod.
/// </summary>
public static class Harmony
{
	/// <summary>
	/// Initialization of the Harmony patching of the mod.
	/// </summary>
	public static void Initialize()
	{
		try
		{
			var harmonyInstance = new HarmonyLib.Harmony(Mod.PackageId);
			harmonyInstance.PatchAll();
		}
		catch (Exception exception)
		{
			Report.Error("Harmony patching failed:");
			Report.Error($"{exception}");
		}
	}
}