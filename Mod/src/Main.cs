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
			if (JLib.playerHealth == null) return;

			if (!enableRegen.Value && JLib.playerHealth.regenRoutine != null)
			{
				JLib.playerHealth.StopCoroutine(JLib.playerHealth.regenRoutine);
			}

			if (!enableVignette.Value && JLib.playerHealth.vignetteRoutine != null)
			{
				JLib.playerHealth.StopCoroutine(JLib.playerHealth.vignetteRoutine);
			}
		}
	}
}
