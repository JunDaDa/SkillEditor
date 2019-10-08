﻿using UnityEditorInternal;
using UnityEngine;
using System.IO;
using StringComparison = System.StringComparison;

public static class SkillEditorConfig {

    // Common
    private static readonly int ProjectPathSubIndex = Application.dataPath.IndexOf("Assets", StringComparison.Ordinal);
    private static readonly string ProjectPath = Application.dataPath.Substring(0, ProjectPathSubIndex);

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
            m_clipFullPath = Path.Combine(modelFileGroupFullPath, "models");
            m_controllerPath = Path.Combine(modelFileGroupFullPath, "animatorcontroller");
        }
        get { return m_prefabFullPath; }
    }
    public static string ClipGroupFullPath => m_clipFullPath;
    public static string ControllerPath => m_controllerPath;

    // Scene
    private const string ScenePath = "Assets/Editor/Scene";
    private const string EditSceneName = "EditScene";
    private const string ExitSceneName = "EditScene";
    public const string SceneExtension = "unity";
    public static readonly string EditScenePath = Path.Combine(ScenePath, SkillEditorTool.FileWithExtension(EditSceneName, SceneExtension));
    public static readonly string ExitScenePath = Path.Combine(ScenePath, SkillEditorTool.FileWithExtension(ExitSceneName, SceneExtension));

    // Layout
    private const string LayoutMenuPath = "Window/Layouts";
    private const string SkillEditorLayoutName = "SkillEditor";
    private const string LayoutExtension = "wlt";
    private static readonly string SkillEditorLayoutFullName = SkillEditorTool.FileWithExtension(SkillEditorLayoutName, LayoutExtension);
    public static readonly string SkillEditorMenuPath = Path.Combine(LayoutMenuPath, SkillEditorLayoutName);
    private static readonly string LayoutFileGroupPath =
#if UNITY_EDITOR_WIN
    Path.Combine(InternalEditorUtility.unityPreferencesFolder, "Layouts");
#elif UNITY_EDITOR_OSX
    Path.Combine(InternalEditorUtility.unityPreferencesFolder, "Layouts/default");
#else
    string.Empty;
#endif
    public static readonly string SkillEditorLayoutFilePath = Path.Combine(LayoutFileGroupPath, SkillEditorLayoutFullName);
    private static readonly string LocalLayoutFileGroupPath = Path.Combine(ProjectPath, "Layout");
    public static readonly string LocalSkillEditorLayoutFilePath = Path.Combine(LocalLayoutFileGroupPath, SkillEditorLayoutFullName);
    private const string ExitSkillEditorLayoutName = "Default";
    public static readonly string ExitLayoutMenuPath = Path.Combine(LayoutMenuPath, ExitSkillEditorLayoutName);

    // Animation
    public const short DefaultAnimationClipLength = 8;

    // Window
    public const short DefaultGUIStyleCount = 8;
    public const short DefaultWindowButtonCount = 8;

    public static void Reset() {
        m_prefabFullPath = string.Empty;
        m_clipFullPath = string.Empty;
        m_controllerPath = string.Empty;
    }
}