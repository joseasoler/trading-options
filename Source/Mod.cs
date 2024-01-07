using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using TO.Mod;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace TradingOptions
{
	/// <summary>
	/// Loads mod settings and displays the mod settings window.
	/// </summary>
	public class Mod : Verse.Mod
	{
		public static readonly string PackageId = "joseasoler.TradingOptions";

		public static readonly string Name = "Trading Options";


		/// <summary>
		/// Reads and initializes mod settings.
		/// </summary>
		/// <param name="content">Content pack data of this mod.</param>
		public Mod(ModContentPack content) : base(content)
		{
			Harmony.Initialize();
			LongEventHandler.ExecuteWhenFinished(InitializeWhenLoadingFinished);
		}

		/// <summary>
		/// Initialization steps that must be taken after the game has finished loading.
		/// </summary>
		private void InitializeWhenLoadingFinished()
		{
			GetSettings<Settings>();
			// Caches which require DefDatabase being fully initialized.
			StorytellerCompPatcher.Patch();
		}

		/// <summary>
		/// Save settings and update the storyteller comp patcher.
		/// </summary>
		public override void WriteSettings()
		{
			base.WriteSettings();
			StorytellerCompPatcher.Patch();
		}

		/// <summary>
		/// Name of the mod in the settings list.
		/// </summary>
		/// <returns>Name of the mod in the settings list.</returns>
		public override string SettingsCategory()
		{
			return SettingsWindow.SettingsCategory();
		}

		/// <summary>
		/// Contents of the mod settings window.
		/// </summary>
		/// <param name="inRect">Available area for drawing the settings.</param>
		public override void DoSettingsWindowContents(Rect inRect)
		{
			SettingsWindow.DoWindowContents(inRect, this);
			base.DoSettingsWindowContents(inRect);
		}
	}
}