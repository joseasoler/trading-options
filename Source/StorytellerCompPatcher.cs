using System.Collections.Generic;
using RimWorld;
using TO.Mod;
using UnityEngine;
using Verse;

namespace TO
{
	/// <summary>
	/// Orbital trader arrival properties for non-random storytellers: StorytellerCompProperties_OnOffCycle.
	/// </summary>
	internal struct OrbitalTraderOnOffCycle
	{
		/// <summary>
		/// Number of days in which the event may randomly happen.
		/// </summary>
		internal readonly float onDays;

		/// <summary>
		/// Number of days in which the event will not happen.
		/// </summary>
		internal readonly float offDays;

		/// <summary>
		/// Number of times the event can happen during onDays.
		/// </summary>
		internal readonly FloatRange numIncidentsRange;

		/// <summary>
		/// Construct the object from existing values to backup the default settings of a storyteller.
		/// </summary>
		/// <param name="on">Number of days in which the event may randomly happen.</param>
		/// <param name="off">Number of days in which the event will not happen.</param>
		/// <param name="range">Number of times the event can happen during onDays.</param>
		public OrbitalTraderOnOffCycle(float on, float off, FloatRange range)
		{
			onDays = on;
			offDays = off;
			numIncidentsRange = range;
		}

		/// <summary>
		/// Calculates properties from the values of FrequencyTime and FrequencyAmount.
		/// onDays and offDays are equal to half of the FrequencyTime (period)
		/// When the period is exactly one, onDays is 1 and offDays is 0 instead.
		/// </summary>
		/// <param name="periodOrbital">Total days of the period between orbital trader arrivals.</param>
		public OrbitalTraderOnOffCycle(int periodOrbital, int traderAmount)
		{
			if (periodOrbital <= 1)
			{
				onDays = 1f;
				offDays = 0f;
			}
			else
			{
				onDays = periodOrbital / 2.0f;
				offDays = onDays;
			}

			numIncidentsRange = new FloatRange(traderAmount, traderAmount);
		}
	}

	/// <summary>
	/// Responsible for patching StorytellerDefs after settings are changed.
	/// </summary>
	[StaticConstructorOnStartup]
	public static class StorytellerCompPatcher
	{
		private static Dictionary<ushort, OrbitalTraderOnOffCycle> _orbitalTraderBackup =
			new Dictionary<ushort, OrbitalTraderOnOffCycle>();

		static StorytellerCompPatcher()
		{
			Patch();
		}

		public static void Patch()
		{
			var time = Settings.GetFrequencyTime(TraderKindCategory.Orbital);
			var amount = Settings.GetFrequencyAmount(TraderKindCategory.Orbital);

			var patchOrbitalTraders = time > 0 && amount > 0;
			var hasOrbitalTraderBackup = _orbitalTraderBackup.Count != 0;

			if (!patchOrbitalTraders && !hasOrbitalTraderBackup)
			{
				return;
			}

			var orbitalTraderValues = new OrbitalTraderOnOffCycle(time, amount);
			foreach (var def in DefDatabase<StorytellerDef>.AllDefs)
			{
				foreach (var comp in def.comps)
				{
					if (comp.GetType() == typeof(StorytellerCompProperties_OnOffCycle))
					{
						var onOffComp = (StorytellerCompProperties_OnOffCycle) comp;
						if (onOffComp.incident == null || onOffComp.incident != IncidentDefOf.OrbitalTraderArrival)
						{
							continue;
						}

						if (!patchOrbitalTraders)
						{
							// When orbital traders are set to default, restore the backup if there is one.
							if (!_orbitalTraderBackup.ContainsKey(def.shortHash))
							{
								var errorLog = $"Could not revert orbital trader arrival changes for {def.defName}";
								Log.ErrorOnce(errorLog, errorLog.GetHashCode());
								continue;
							}

							orbitalTraderValues = _orbitalTraderBackup[def.shortHash];
						}
						else if (!hasOrbitalTraderBackup)
						{
							// Store a backup in case that settings get reverted later.
							_orbitalTraderBackup[def.shortHash] = new OrbitalTraderOnOffCycle(onOffComp.onDays, onOffComp.offDays,
								onOffComp.numIncidentsRange);
						}

						onOffComp.onDays = orbitalTraderValues.onDays;
						onOffComp.offDays = orbitalTraderValues.offDays;
						onOffComp.numIncidentsRange = orbitalTraderValues.numIncidentsRange;
						break;
					}
				}
			}

			if (Find.Storyteller != null)
			{
				Find.Storyteller.Notify_DefChanged();
			}
		}
	}
}