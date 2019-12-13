﻿using UnityEditorInternal;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

namespace SkillEditor {

    public static class Config {

        static Config() => ReloadConfigJson();
        public static void ReloadConfigJson() {
            m_configJson = JsonUtility.FromJson<ConfigJson>(File.ReadAllText(ConfigJsonPath));
            ListUseAutoSeedEffect.Clear();
            if (m_configJson.use_autoseed_effect == null || m_configJson.use_autoseed_effect.Length == 0)
                return;
            foreach (string jsonString in m_configJson.use_autoseed_effect)
                ListUseAutoSeedEffect.Add(JsonUtility.FromJson<PrefabChildName>(jsonString));
        }

        #region Config
        
        public const short ErrorIndex = -1;
        // Path
        private static string ConfigJsonPath = Tool.CombinePath(Application.dataPath, "Editor/SkillEditor/.Config.json");
        private static readonly int ProjectPathSubIndex = Application.dataPath.IndexOf("Assets");
        public static readonly string ProjectPath = Application.dataPath.Substring(0, ProjectPathSubIndex);

        // Extension
        public const string MetaExtension = "meta";
        public const string ClipLowerExtension = "fbx";
        public static string ClipUpperExtension = ClipLowerExtension.ToUpper();
        public const string PrefabExtension = "prefab";
        public const string AnimatorControllerExtension = "controller";
        private const string LayoutExtension = "wlt";
        public const string SceneExtension = "unity";

        // Symbol
        public const string AnimationClipSymbol = "@";  

        // Weapon
        public const string WeaponParentNode = "R_Weapon_Point";

        // Draw Cube
        public const string DrawCubeNodeName = "FootCenter_Point00";

        // Layout
        private const string LayoutMenuPath = "Window/Layouts";
        private const string EditorLayoutName = "SkillEditor";
        private static readonly string EditorLayoutFullName = Tool.FileWithExtension(EditorLayoutName, LayoutExtension);
        public static readonly string MenuPath = Tool.CombinePath(LayoutMenuPath, EditorLayoutName);
        private static readonly string LayoutFileGroupPath =
#if UNITY_EDITOR_WIN
            Tool.CombinePath(InternalEditorUtility.unityPreferencesFolder, "Layouts");
#elif UNITY_EDITOR_OSX
            Tool.CombinePath(InternalEditorUtility.unityPreferencesFolder, "Layouts/default");
#else
            string.Empty;
#endif
        public static readonly string EditorLayoutFilePath = Tool.CombinePath(LayoutFileGroupPath, EditorLayoutFullName);
        private static readonly string LocalLayoutFileGroupPath = Tool.CombinePath(Application.dataPath, "Editor/.Layout");
        public static readonly string LocalEditorLayoutFilePath = Tool.CombinePath(LocalLayoutFileGroupPath, EditorLayoutFullName);
        private const string ExitEditorLayoutName = "Default";
        public static readonly string ExitLayoutMenuPath = Tool.CombinePath(LayoutMenuPath, ExitEditorLayoutName);

        // Scene
        private const string ScenePath = "Assets/Editor/Scene";
        private const string EditSceneName = "EditScene";
        private const string ExitSceneName = "EditScene";
        public static readonly string EditScenePath = Tool.CombinePath(ScenePath, Tool.FileWithExtension(EditSceneName, SceneExtension));
        public static readonly string ExitScenePath = Tool.CombinePath(ScenePath, Tool.FileWithExtension(ExitSceneName, SceneExtension));
        #endregion

        #region Config Json

        // Runtime    
        public static ushort FrameCount => m_configJson.frame_rate;
        public static float FramesPerSecond => 1f / FrameCount;
        public static float RuntimeEffectDelay => m_configJson.runtime_effect_delay * FramesPerSecond;
        public static float RuntimeCubeDelay => m_configJson.runtime_draw_cube_delay * FramesPerSecond;
        public static float DrawCubeLastTime => m_configJson.runtime_draw_cube_delay * FramesPerSecond;

        // Path
        public static string AnimatorControllerFolder => m_configJson.animator_controller_folder;
        public static string ModelPrefabPath => m_configJson.model_path;
        public static string WeaponPath => Tool.CombinePath(ProjectPath, m_configJson.weapon_path);
        public static string AnimationClipFolder => m_configJson.clip_folder;
        public static string ModelPrefabFolder => m_configJson.prefab_folder;

        // Prefix
        public static string ModelPrefabPrefix => m_configJson.hero_prefix;
        public static string WeaponFilePrefix => m_configJson.weapon_prefix;

        // Effect
        public static string[] SkillEffectPath => m_configJson.skill_effect_path;
        public static string[] EffectExcluteComponents => m_configJson.effect_exclute_component;
        public static List<PrefabChildName> ListUseAutoSeedEffect = new List<PrefabChildName>();

        private static ConfigJson m_configJson;

        private struct ConfigJson {
 
            public ushort frame_rate;
            public short runtime_effect_delay;
            public short runtime_draw_cube_delay;
            public ushort draw_cube_last_time;
            public string model_path;
            public string weapon_path;
            public string clip_folder;
            public string prefab_folder;
            public string hero_prefix;
            public string weapon_prefix;
            public string animator_controller_folder;
            public string[] skill_effect_path;
            public string[] effect_exclute_component;
            public string[] use_autoseed_effect;

            public ConfigJson(ushort arg) {
                // unity warnning
                frame_rate = draw_cube_last_time = 0;
                runtime_effect_delay = runtime_draw_cube_delay = 0;
                model_path = weapon_path = clip_folder = prefab_folder = hero_prefix = weapon_prefix = animator_controller_folder = null;
                skill_effect_path = effect_exclute_component = use_autoseed_effect = null;
            }
        }

        public struct PrefabChildName {

            public string prefabName;
            public string[] childName;
        }
        #endregion
    }
}