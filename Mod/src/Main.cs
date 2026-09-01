using MelonLoader;
using UnityEngine;
using jlib;

namespace healthregentoggle
{
	public class HealthRegenToggle : MelonMod
	{
		public const string Version = "1.3.0";
		
		private MelonPreferences_Entry<bool> enableRegen;
		private MelonPreferences_Entry<bool> enableVignette;
		
		public override void OnInitializeMelon()
		{
			var menu = JLib.Register("HealthRegenToggle", Color.green);

			enableRegen = menu.Bool("Enable Regeneration", true, Color.yellow);
			enableVignette = menu.Bool("Enable Damage Vignette", true, Color.yellow);
		}

		public override void OnUpdate()
		{
			var playerHealth = JLib.playerHealth;
			if (playerHealth == null) return;

			if (!enableRegen.Value && playerHealth.regenRoutine != null)
			{
				playerHealth.StopCoroutine(playerHealth.regenRoutine);
			}

			if (!enableVignette.Value && playerHealth.vigRend != null)
			{
				playerHealth.vigRend.enabled = !playerHealth.alive; // Enable vignette on death to fix respawning
			}
		}
	}
}
