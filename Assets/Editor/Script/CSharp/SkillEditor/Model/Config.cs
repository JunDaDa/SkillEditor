﻿using UnityEditorInternal;
using UnityEngine;
using StringComparison = System.StringComparison;

namespace SkillEditor {

    internal static class Config {

        // Common
        private static readonly int ProjectPathSubIndex = Application.dataPath.IndexOf("Assets", StringComparison.Ordinal);
        public static readonly string ProjectPath = Application.dataPath.Substring(0, ProjectPathSubIndex);

        // Prefab Group Structure
        public const string ModelPrefabPath = "Assets/Editor/Asset/prefabs";
        public const string ModelPrefabExtension = "prefab";
        public const string FilePanelTitle = "模型预设路径";
        private static string m_prefabFullPath = string.Empty;
        private static string m_clipFullPath = string.Empty;
        private static string m_controllerPath = string.Empty;
        public static string PrefabPath {
            set {
                m_prefabFullPath = value;
                if (m_prefabFullPath == string.Empty)
                    return;
                int subIndex = m_prefabFullPath.IndexOf("prefabs/", StringComparison.Ordinal);
                string modelFileGroupFullPath = m_prefabFullPath.Substring(0, subIndex);
                m_clipFullPath = Tool.CombinePath(modelFileGroupFullPath, "models");
                m_controllerPath = Tool.CombinePath(modelFileGroupFullPath, "animatorcontroller");
                m_controllerPath = Tool.FullPathToProjectPath(m_controllerPath);
            }
            get { return m_prefabFullPath; }
        }
        public static string ClipGroupFullPath => m_clipFullPath;

        // Scene
        private const string ScenePath = "Assets/Editor/Scene";
        private const string EditSceneName = "EditScene";
        private const string ExitSceneName = "EditScene";
        public const string SceneExtension = "unity";
        public static readonly string EditScenePath = Tool.CombinePath(ScenePath, Tool.FileWithExtension(EditSceneName, SceneExtension));
        public static readonly string ExitScenePath = Tool.CombinePath(ScenePath, Tool.FileWithExtension(ExitSceneName, SceneExtension));

        // Layout
        private const string LayoutMenuPath = "Window/Layouts";
        private const string SkillEditorLayoutName = "SkillEditor";
        private const string LayoutExtension = "wlt";
        private static readonly string SkillEditorLayoutFullName = Tool.FileWithExtension(SkillEditorLayoutName, LayoutExtension);
        public static readonly string SkillEditorMenuPath = Tool.CombinePath(LayoutMenuPath, SkillEditorLayoutName);
        private static readonly string LayoutFileGroupPath =
    #if UNITY_EDITOR_WIN
        Tool.CombinePath(InternalEditorUtility.unityPreferencesFolder, "Layouts");
    #elif UNITY_EDITOR_OSX
        Tool.CombinePath(InternalEditorUtility.unityPreferencesFolder, "Layouts/default");
    #else
        string.Empty;
    #endif
        public static readonly string SkillEditorLayoutFilePath = Tool.CombinePath(LayoutFileGroupPath, SkillEditorLayoutFullName);
        private static readonly string LocalLayoutFileGroupPath = Tool.CombinePath(Application.dataPath, "Editor/.Layout");
        public static readonly string LocalSkillEditorLayoutFilePath = Tool.CombinePath(LocalLayoutFileGroupPath, SkillEditorLayoutFullName);
        private const string ExitSkillEditorLayoutName = "Default";
        public static readonly string ExitLayoutMenuPath = Tool.CombinePath(LayoutMenuPath, ExitSkillEditorLayoutName);

        // Animation
        private const string AnimatorControllerExtension = "controller";
        private const string AnimatorControllerCopyName = "Copy";
        private static readonly string AnimatorControllerCopyPath = LocalLayoutFileGroupPath;
        public const short DefaultAnimationClipLength = 8;

        public static void Reset() {
            m_prefabFullPath = string.Empty;
            m_clipFullPath = string.Empty;
            m_controllerPath = string.Empty;
        }

        public static string GetAnimatorControllerPath(string fileName) {
            return Tool.CombineFilePath(m_controllerPath, fileName, AnimatorControllerExtension);
        }

        public static string GetAnimatorControllerCopyPath(string fileName) {
            return Tool.CombineFilePath(AnimatorControllerCopyPath, fileName + AnimatorControllerCopyName, AnimatorControllerExtension);
        }
    }
}