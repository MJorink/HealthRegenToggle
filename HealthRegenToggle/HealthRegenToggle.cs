using MelonLoader;
using BoneLib;
using BoneLib.BoneMenu;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[assembly: MelonInfo(typeof(HealthRegenToggle.Core), "HealthRegenToggle", "1.0.2", "jorink")]
[assembly: MelonGame("Stress Level Zero", "BONELAB")]

namespace HealthRegenToggle {
    public class Core : MelonMod {

        internal static Core Instance;
        private bool lastRegenEnabled;
        
        MelonPreferences_Category category;
        MelonPreferences_Entry<bool> RegenEntry;

        public override void OnInitializeMelon() {
            Instance = this;
            SetupMelonPreferences();
            lastRegenEnabled = RegenEntry.Value;
            SetupBoneMenu();
            PatchRegenMethods();
        }

        public override void OnUpdate() {
            if (RegenEntry == null)
                return;

            bool currentEnabled = RegenEntry.Value;
            if (currentEnabled == lastRegenEnabled)
                return;

            lastRegenEnabled = currentEnabled;
        }

        private void SetupBoneMenu() {
            Page defaultPage = Page.Root.CreatePage("Jorink", Color.red).CreatePage("HealthRegenToggle", Color.green);

            defaultPage.CreateBool("Enable Health Regen", Color.blue, RegenEntry.Value, (a) => { RegenEntry.Value = a; });

            defaultPage.CreateFunction("Save Settings", Color.cyan, () => { MelonPreferences.Save(); });                   
        }

        private void SetupMelonPreferences() {
            category = MelonPreferences.CreateCategory("HealthRegenToggle");

            RegenEntry = category.CreateEntry("Health Regen", true);

            MelonPreferences.Save();
            category.SaveToFile();
        }

        private void PatchRegenMethods() {
            MethodInfo prefix = typeof(Core).GetMethod(nameof(AllowHealthRegen), BindingFlags.Static | BindingFlags.NonPublic);
            const string typeName = "Il2CppSLZ.Marrow.Player_Health";
            const string regenCoroutineName = "CoRegenHealth";

            var patchedMethods = new HashSet<string>();

            var type = TryResolveType(typeName);
            if (type == null) {
                MelonLogger.Warning($"Could not resolve target type {typeName}.");
                return;
            }

            foreach (var nestedType in type.GetNestedTypes(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)) {
                if (!nestedType.Name.Contains(regenCoroutineName))
                    continue;

                var moveNext = nestedType.GetMethod("MoveNext", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (moveNext == null)
                    continue;

                string methodKey = GetMethodKey(moveNext);
                if (!patchedMethods.Add(methodKey))
                    continue;

                try {
                    HarmonyInstance.Patch(moveNext, prefix: new HarmonyMethod(prefix));
                }
                catch (Exception ex) {
                    MelonLogger.Warning($"Failed to patch coroutine {nestedType.FullName}.MoveNext: {ex.GetType().Name}: {ex.Message}");
                }
            }
        }

        private static string GetMethodKey(MethodInfo method) {
            return $"{method.DeclaringType?.FullName ?? "<null>"}::{method}";
        }

        private static Type TryResolveType(string fullTypeName) {
            var type = Type.GetType(fullTypeName, throwOnError: false);
            if (type != null)
                return type;

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                type = assembly.GetType(fullTypeName, throwOnError: false, ignoreCase: false);
                if (type != null)
                    return type;
            }

            return null;
        }

        private static bool AllowHealthRegen() {
            if (Instance == null)
                return true;

            return Instance.RegenEntry.Value;
        }        
    }
}
