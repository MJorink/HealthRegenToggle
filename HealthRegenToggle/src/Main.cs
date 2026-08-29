using MelonLoader;
using BoneLib;
using BoneLib.BoneMenu;
using Il2CppSLZ.Marrow;
using UnityEngine;

namespace HealthRegenToggle
{
    public class HealthRegenToggleMod : MelonMod
    {
    	public const string Title = "HealthRegenToggle";
    	public const string Description = "A BoneLab mod that allows you to toggle health regeneration.";
    	public const string Version = "1.2.0";

    	private static MelonPreferences_Entry<bool> enableRegen;
    	private static MelonPreferences_Entry<bool> enableVignette;

        private static Player_Health playerHealth;

        public override void OnInitializeMelon()
        {
            SetupMelonPreferences();
            SetupBoneMenu();
        }

        private void SetupMelonPreferences()
        {
            var category = MelonPreferences.CreateCategory("HealthRegenToggle");
            
            enableRegen = category.CreateEntry("Health Regeneration", true);
            enableVignette = category.CreateEntry("Health Vignette", true);

            MelonPreferences.Save();
        }

        private void SetupBoneMenu()
        {
            Page defaultPage = Page.Root.CreatePage("Jorink", Color.red).CreatePage("HealthRegenToggle", Color.green);

            defaultPage.CreateBool("Health Regeneration", Color.yellow, enableRegen.Value, (value) => { enableRegen.Value = value; });
            defaultPage.CreateBool("Health Vignette", Color.yellow, enableVignette.Value, (value) => { enableVignette.Value = value; });
            defaultPage.CreateFunction("Save Settings", Color.green, () => MelonPreferences.Save());
        }

        public override void OnUpdate()
        {
        	if (!playerHealth)
        	{
        		playerHealth = Player.RigManager.health.TryCast<Player_Health>();
        		return;
        	}
        	
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
