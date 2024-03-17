using RimWorld;

namespace TradingOptions
{
	[DefOf]
	public class IncidentCategoriesDefOf
	{
		public static IncidentCategoryDef OrbitalVisitor;
		public static IncidentCategoryDef FactionArrival;

		static IncidentCategoriesDefOf()
		{
			DefOfHelper.EnsureInitializedInCtor(typeof (IncidentCategoriesDefOf));
		}
	}
}