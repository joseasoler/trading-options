using System.Collections.Generic;
using HarmonyLib;
using TO.Mod;
using Verse;

namespace TradingOptions.Patches.CaravanVisitorDepartureTime
{
	public static class Helper
	{
		/// <summary>
		/// Replacement for Rand.Range with a custom range defined through mod settings.
		/// </summary>
		/// <param name="min">Original minimum value.</param>
		/// <param name="max">Original maximum value.</param>
		/// <returns>Random departure time in the settings range.</returns>
		public static int GetTime(int min, int max)
		{
			var departureTime = Settings.GetDepartureTime(TraderKindCategory.Caravan);
			return departureTime == 0 ? Rand.Range(min, max) : departureTime;
		}

		/// <summary>
		/// Replaces Rand.Range with a custom range defined through mod settings.
		/// </summary>
		/// <param name="patch">Harmony patch file calling this helper.</param>
		/// <param name="instructions">Instructions to patch.</param>
		/// <returns>Patched instructions.</returns>
		public static IEnumerable<CodeInstruction> InjectDepartureTimeIntoRandRange(string patch,
			IEnumerable<CodeInstruction> instructions)
		{
			const int expectedReplacements = 1;
			int replacements = 0;

			foreach (var code in instructions)
			{
				if (code.Calls(Methods.Verse_Rand_Range))
				{
					++replacements;
					yield return CodeInstruction.Call(typeof(Helper), nameof(Helper.GetTime));
				}
				else
				{
					yield return code;
				}
			}

			if (replacements != expectedReplacements)
			{
				Report.Error(
					$"{patch}: Incorrect number of Verse_Rand_Range replacements. Expected {expectedReplacements}, made {replacements}.");
			}
		}
	}
}