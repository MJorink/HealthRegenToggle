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

    	private static MelonPreferences_Entry<bool> regenEntry;

        private static Player_Health playerHealth;
        private static RigManager rig;
        private static Coroutine regenRoutine;

        public override void OnInitializeMelon()
        {
            SetupMelonPreferences();
            SetupBoneMenu();
            SetupHooks();
        }

        private void SetupMelonPreferences()
        {
            var category = MelonPreferences.CreateCategory("HealthRegenToggle");
            
            regenEntry = category.CreateEntry("Health Regeneration", true);
            
            MelonPreferences.Save();
            category.SaveToFile();
        }

        private void SetupBoneMenu()
        {
            Page defaultPage = Page.Root.CreatePage("Jorink", Color.red).CreatePage("HealthRegenToggle", Color.green);

            defaultPage.CreateBool("Health Regeneration", Color.blue, regenEntry.Value, (value) => { regenEntry.Value = value; });
            defaultPage.CreateFunction("Save Settings", Color.cyan, () => MelonPreferences.Save());
        }

        private static void SetupHooks()
        {
        	Hooking.OnLevelLoaded += OnLevelLoaded;
        }

        private static void OnLevelLoaded(LevelInfo levelInfo)
        {
        	rig = Player.RigManager;
        	playerHealth = rig.health.TryCast<Player_Health>();
        	regenRoutine = playerHealth.regenRoutine;
        }

        private static bool IsModAllowed()
        {
        	if (!rig || !playerHealth) return false;
        	
        	if (!regenEntry.Value && playerHealth.regenerating && regenRoutine != null) return true;
        	return false;
        }

        public override void OnUpdate()
        {
        	if (!IsModAllowed()) return;
        	playerHealth.StopCoroutine(regenRoutine);
        	playerHealth.regenerating = false;
        }
    }
}
