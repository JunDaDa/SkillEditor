﻿using UnityEditor;

public static class SkillEditorMenu {

	[MenuItem("技能编辑器/选择模型 Prefab")]
    private static void OpenPrefab() {
        SkillEditorManager.OpenPrefab();
	}

    [MenuItem("技能编辑器/打开 Timeline 窗口")]
    private static void OpenTimelineWindow() {
        SkillEditorWindow.Open();
	}

	[MenuItem("技能编辑器/退出编辑器模式")]
    private static void ExitSkillEditor() {
        SkillEditorManager.RevertScene();
    }
}