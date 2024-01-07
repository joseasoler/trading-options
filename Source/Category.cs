using System.Collections.Generic;
using RimWorld;
using TradingOptions;
using Verse;

namespace TradingOptions;

/// <summary>
/// Helper functions.
/// </summary>
public static class Category
{
	/// <summary>
	/// The category of some mod traders cannot be found with the default TO algorithm.
	/// They are assigned hardcoded categories here.
	/// </summary>
	private static readonly Dictionary<string, TraderKindCategory> HardcodedTraders =
		new()
		{
			{ "Orbital_Rimatomics", TraderKindCategory.Orbital }, // Rimatomics
			{ "Base_City", TraderKindCategory.Settlement }, // RimCities
			{ "ExC_NormalTrader", TraderKindCategory.Orbital } // Udon.ExtendTheCat
		};

	/// <summary>
	/// Lazily initialized cache of categories for each trader.
	/// </summary>
	private static readonly Dictionary<ushort, TraderKindCategory>
		CategoryByHash = new();

	/// <summary>
	/// Algorithm for obtaining the category of a trader. Called only when the trader is not in the cache.
	/// </summary>
	/// <param name="def">Trader being evaluated.</param>
	/// <returns>Category of the trader.</returns>
	private static TraderKindCategory GetImpl(TraderKindDef def)
	{
		if (HardcodedTraders.TryGetValue(def.defName, out var value))
		{
			return value;
		}

		if (def.orbital)
		{
			return TraderKindCategory.Orbital;
		}

		foreach (var factionDef in DefDatabase<FactionDef>.AllDefsListForReading)
		{
			if (factionDef.baseTraderKinds.Contains(def)) return TraderKindCategory.Settlement;
			if (factionDef.caravanTraderKinds.Contains(def)) return TraderKindCategory.Caravan;
			if (factionDef.visitorTraderKinds.Contains(def)) return TraderKindCategory.Visitor;
		}

		Report.ErrorOnce($"Failed to find TraderKindCategory of trader {def.defName}!");

		return TraderKindCategory.None;
	}

	/// <summary>
	/// Finds out the trader kind category of a trader.
	/// </summary>
	/// <param name="def">Trader being evaluated.</param>
	/// <returns>Trader category.</returns>
	public static TraderKindCategory Get(TraderKindDef def)
	{
		if (def == null)
		{
			// Null defs can happen with mods such as RimCities.
			return TraderKindCategory.None;
		}

		if (CategoryByHash.TryGetValue(def.shortHash, out var categoryByHash))
		{
			return categoryByHash;
		}

		var category = GetImpl(def);
		CategoryByHash[def.shortHash] = category;
		return category;
	}
}