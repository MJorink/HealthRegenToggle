using MelonLoader;
using UnityEngine;
using jlib;

namespace HealthRegenToggle
{
	public class HRT : MelonMod
	{
		public const string Version = "1.3.0";
		
		private static MelonPreferences_Entry<bool> enableRegen;
		private static MelonPreferences_Entry<bool> enableVignette;
		
		public override void OnInitializeMelon()
		{
			// MelonPreferences
			var category = MelonPreferences.CreateCategory("HealthRegenToggle");
			category.SetFilePath("Jorink/HealthRegenToggle.cfg");
			
			enableRegen = category.CreateEntry<bool>("Enable Regeneration", true);
			enableVignette = category.CreateEntry<bool>("Enable Damage Vignette", true);
			
			category.SaveToFile();

			// BoneMenu Page
			var modPage = JLib.rootPage.CreatePage("HealthRegenToggle", Color.green);
			
			modPage.CreateBool("Health Regeneration", Color.yellow, enableRegen.Value, (value) => { enableRegen.Value = value; });
			modPage.CreateBool("Damage Vignette", Color.yellow, enableVignette.Value, (value) => { enableVignette.Value = value; });
		}

		public override void OnUpdate()
		{
			var playerHealth = JLib.playerHealth;
			if (playerHealth == null) return;

			if (!enableRegen.Value && playerHealth.regenRoutine != null)
			{
				playerHealth.StopCoroutine(playerHealth.regenRoutine);
			}

			if (!enableVignette.Value && playerHealth.vignetteRoutine != null)
			{
				playerHealth.StopCoroutine(playerHealth.vignetteRoutine);
			}
		}
	}
}
