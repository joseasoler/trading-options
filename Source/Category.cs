using System.Collections.Generic;
using RimWorld;
using Verse;

namespace TO
{
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
			new Dictionary<string, TraderKindCategory>
			{
				// Rimatomics
				{ "Orbital_Rimatomics", TraderKindCategory.Orbital },
				// RimCities
				{ "Base_City", TraderKindCategory.Settlement },
				// Udon.ExtendTheCat
				{ "ExC_NormalTrader", TraderKindCategory.Orbital }
			};

		/// <summary>
		/// Lazily initialized cache of categories for each trader.
		/// </summary>
		private static readonly Dictionary<ushort, TraderKindCategory>
			CategoryByHash = new Dictionary<ushort, TraderKindCategory>();

		/// <summary>
		/// Algorithm for obtaining the category of a trader. Called only when the trader is not in the cache.
		/// </summary>
		/// <param name="def">Trader being evaluated.</param>
		/// <returns>Category of the trader.</returns>
		private static TraderKindCategory GetImpl(TraderKindDef def)
		{
			if (HardcodedTraders.ContainsKey(def.defName))
			{
				return HardcodedTraders[def.defName];
			}

			if (def.orbital)
			{
				return TraderKindCategory.Orbital;
			}

			foreach (var factionDef in DefDatabase<FactionDef>.AllDefs)
			{
				if (factionDef.baseTraderKinds.Contains(def)) return TraderKindCategory.Settlement;
				if (factionDef.caravanTraderKinds.Contains(def)) return TraderKindCategory.Caravan;
				if (factionDef.visitorTraderKinds.Contains(def)) return TraderKindCategory.Visitor;
			}

			var str = $"[TradingOptions] Failed to find TraderKindCategory of trader {def.defName}!";
			Log.ErrorOnce(str, str.GetHashCode());

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

			if (CategoryByHash.ContainsKey(def.shortHash))
			{
				return CategoryByHash[def.shortHash];
			}

			var category = GetImpl(def);
			CategoryByHash[def.shortHash] = category;
			return category;
		}
	}
}