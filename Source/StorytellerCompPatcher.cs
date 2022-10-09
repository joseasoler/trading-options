using System;
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
	internal struct OrbitalOnOffCycle
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
		public OrbitalOnOffCycle(float on, float off, FloatRange range)
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
		/// <param name="traderAmount">Number of orbital traders during that period.</param>
		public OrbitalOnOffCycle(int periodOrbital, int traderAmount)
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
	/// Trader arrival properties for caravans and visitors in non-random storytellers:
	/// StorytellerCompProperties_FactionInteraction.
	/// </summary>
	internal struct FactionInteraction
	{
		/// <summary>
		/// Minimum number of days between events.
		/// </summary>
		internal readonly float minSpacingDays;

		/// <summary>
		/// Number of times the event happens in a year.
		/// </summary>
		internal readonly float baseIncidentsPerYear;

		/// <summary>
		/// Construct the object from existing values to backup the default settings of a storyteller.
		/// </summary>
		/// <param name="spacingDays">Minimum number of days between events.</param>
		/// <param name="incidentsPerYear">Number of times the event happens in a year.</param>
		public FactionInteraction(float spacingDays, float incidentsPerYear)
		{
			minSpacingDays = spacingDays;
			baseIncidentsPerYear = incidentsPerYear;
		}
	}

	/// <summary>
	/// Responsible for patching StorytellerDefs after settings are changed.
	/// </summary>
	[StaticConstructorOnStartup]
	public static class StorytellerCompPatcher
	{
		private static readonly Dictionary<ushort, FactionInteraction> CaravanBackup =
			new Dictionary<ushort, FactionInteraction>();

		private static readonly Dictionary<ushort, OrbitalOnOffCycle> OrbitalBackup =
			new Dictionary<ushort, OrbitalOnOffCycle>();

		private static readonly Dictionary<ushort, FactionInteraction> VisitorBackup =
			new Dictionary<ushort, FactionInteraction>();

		static StorytellerCompPatcher()
		{
			Patch();
		}

		private static void PatchCompOnOffCycle(StorytellerDef def, StorytellerCompProperties comp)
		{
			var onOffComp = (StorytellerCompProperties_OnOffCycle) comp;
			if (onOffComp.incident != IncidentDefOf.OrbitalTraderArrival)
			{
				return;
			}

			var time = Settings.GetFrequencyTime(TraderKindCategory.Orbital);
			var amount = Settings.GetFrequencyAmount(TraderKindCategory.Orbital);
			var shouldPatch = time > 0 && amount > 0;
			var hasBackup = OrbitalBackup.Count != 0;

			if (!shouldPatch && !hasBackup)
			{
				return;
			}

			var newValue = new OrbitalOnOffCycle(time, amount);

			if (!shouldPatch)
			{
				// When orbital traders are set to default, restore the backup if there is one.
				if (!OrbitalBackup.ContainsKey(def.shortHash))
				{
					var errorLog = $"Could not revert orbital trader arrival changes for {def.defName}";
					Log.ErrorOnce(errorLog, errorLog.GetHashCode());
					return;
				}

				newValue = OrbitalBackup[def.shortHash];
			}
			else if (!hasBackup)
			{
				// Store a backup in case that settings get reverted later.
				OrbitalBackup[def.shortHash] = new OrbitalOnOffCycle(onOffComp.onDays, onOffComp.offDays,
					onOffComp.numIncidentsRange);
			}

			onOffComp.onDays = newValue.onDays;
			onOffComp.offDays = newValue.offDays;
			onOffComp.numIncidentsRange = newValue.numIncidentsRange;
		}

		private static void PatchFactionInteraction(StorytellerDef def, StorytellerCompProperties comp,
			TraderKindCategory category)
		{
			var factionComp = (StorytellerCompProperties_FactionInteraction) comp;

			var backup = category == TraderKindCategory.Caravan ? CaravanBackup : VisitorBackup;

			var time = Settings.GetFrequencyTime(category);
			var amount = Settings.GetFrequencyAmount(category);
			var shouldPatch = time > 0 && amount > 0;
			var hasBackup = backup.Count != 0;

			if (!shouldPatch && !hasBackup)
			{
				return;
			}

			var newValue = new FactionInteraction(time, amount);

			if (!shouldPatch)
			{
				// When orbital traders are set to default, restore the backup if there is one.
				if (!backup.ContainsKey(def.shortHash))
				{
					var errorLog =
						$"Could not revert {Enum.GetName(typeof(TraderKindCategory), category)} changes for {def.defName}";
					Log.ErrorOnce(errorLog, errorLog.GetHashCode());
					return;
				}

				newValue = backup[def.shortHash];
			}
			else if (!hasBackup)
			{
				// Store a backup in case that settings get reverted later.
				backup[def.shortHash] = new FactionInteraction(factionComp.minSpacingDays, factionComp.baseIncidentsPerYear);
			}

			// ToDo remove
			factionComp.minDaysPassed = 0;
			factionComp.minSpacingDays = newValue.minSpacingDays;
			factionComp.baseIncidentsPerYear = newValue.baseIncidentsPerYear;
		}

		public static void Patch()
		{
			foreach (var def in DefDatabase<StorytellerDef>.AllDefs)
			{
				foreach (var comp in def.comps)
				{
					if (comp.GetType() == typeof(StorytellerCompProperties_OnOffCycle))
					{
						PatchCompOnOffCycle(def, comp);
					}
					else if (comp.GetType() == typeof(StorytellerCompProperties_FactionInteraction))
					{
						var factionInteractionComp = (StorytellerCompProperties_FactionInteraction) comp;
						if (factionInteractionComp.incident == IncidentDefOf.TraderCaravanArrival)
						{
							PatchFactionInteraction(def, comp, TraderKindCategory.Caravan);
						}
						else if (factionInteractionComp.incident == IncidentDefOf.VisitorGroup)
						{
							PatchFactionInteraction(def, comp, TraderKindCategory.Visitor);
						}
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