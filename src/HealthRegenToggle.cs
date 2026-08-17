using MelonLoader;
using BoneLib;
using BoneLib.BoneMenu;
using Il2CppSLZ.Marrow;
using UnityEngine;

[assembly: MelonInfo(typeof(HealthRegenToggle.Core), "HealthRegenToggle", "1.1.0", "jorink")]
[assembly: MelonGame("Stress Level Zero", "BONELAB")]

namespace HealthRegenToggle {
    public class Core : MelonMod {

        private Player_Health playerHealth;
        MelonPreferences_Entry<bool> regenEntry;

        public override void OnInitializeMelon() {
            SetupMelonPreferences();
            SetupBoneMenu();
        }

        public override void OnUpdate() {
            if (regenEntry.Value) return;

            if (playerHealth == null) {
                playerHealth = UnityEngine.Object.FindObjectOfType<Player_Health>();
                if (playerHealth == null) return;
            }

            if (!playerHealth.regenerating) return;

            var routine = playerHealth.regenRoutine;
            if (routine != null)
                playerHealth.StopCoroutine(routine);

            playerHealth.regenerating = false;
        }

        private void SetupBoneMenu() {
            Page defaultPage = Page.Root.CreatePage("Jorink", Color.red).CreatePage("HealthRegenToggle", Color.green);

            defaultPage.CreateBool("Enable Health Regen", Color.blue, regenEntry.Value, (value) => { regenEntry.Value = value; });

            defaultPage.CreateFunction("Save Settings", Color.cyan, () => MelonPreferences.Save());
        }

        private void SetupMelonPreferences() {
            var category = MelonPreferences.CreateCategory("HealthRegenToggle");
            regenEntry = category.CreateEntry("Health Regen", true);

            MelonPreferences.Save();
        }
    }
}
