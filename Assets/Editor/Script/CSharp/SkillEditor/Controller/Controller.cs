﻿using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

namespace SkillEditor {

    internal static class Controller {

        private static GameObject m_model = null;

        private static List<AnimationClip> m_animationClips = new List<AnimationClip>(Config.DefaultAnimationClipLength);

        private static bool m_isGenericClip;

        private static double m_lastTime;

        public static void Start(string prefabPath) {
            Reset();
            GameObject prefab = PrefabUtility.LoadPrefabContents(prefabPath);
            m_model = Object.Instantiate(prefab);
            m_model.name = prefab.name;
            Selection.activeGameObject = m_model;
            PrefabUtility.UnloadPrefabContents(prefab);
            SetAllAnimationClip();
            AnimationModel.AnimationClips = m_animationClips.ToArray();
            m_isGenericClip = AnimationModel.GenericState();
            InitAnimation();
            EditorScene.RegisterSceneGUI();
            EditorWindow.SetDisplayData(AnimationModel.AnimationClipNames, AnimationModel.AnimationClipIndexs);
            EditorWindow.Open();
        }

        private static void SetAllAnimationClip() {
            string[] fileNames = Directory.GetFiles(Config.ClipGroupFullPath);
            m_animationClips.Clear();
            for (int index = 0; index < fileNames.Length; index++) {
                if (fileNames[index].Contains(".meta") || !fileNames[index].Contains("@") ||
                    !(fileNames[index].Contains(".fbx") || fileNames[index].Contains(".FBX")))
                    continue;
                string path = Tool.FullPathToProjectPath(fileNames[index]);
                AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
                if (clip == null)
                    continue;
                m_animationClips.Add(clip);
            }
        }

        private static void InitAnimation() {
            if (!m_isGenericClip)
                return;
            Animator animator = m_model.GetComponent<Animator>();
            if (animator == null)
                Debug.LogError("Prefab's animator is not exit");
            SkillAnimator.Animator = animator;
            RemoveAllAnimatorTransition();
        }

        private static void RemoveAllAnimatorTransition() {
            string sourcePath = Config.GetAnimatorControllerPath(m_model.name);
            string copyPath = Config.GetAnimatorControllerCopyPath(m_model.name);
            AnimatorController animatorController = AssetDatabase.LoadAssetAtPath<AnimatorController>(sourcePath);
            File.Copy(Tool.ProjectPathToFullPath(sourcePath), copyPath, true);
            AnimatorControllerLayer[] layers = animatorController.layers;
            for (int layerIndex = 0; layerIndex < layers.Length; layerIndex++) {
                AnimatorStateMachine mainMachine = layers[layerIndex].stateMachine;
                if (mainMachine == null || mainMachine.states == null)
                    continue;
                RemoveAnimatorMachineTransition(mainMachine);
                ChildAnimatorStateMachine[] subMachines = mainMachine.stateMachines;
                if (subMachines == null)
                    continue;
                for (int machineIndex = 0; machineIndex < subMachines.Length; machineIndex++) {
                    AnimatorStateMachine subMachine = subMachines[machineIndex].stateMachine;
                    if (subMachine == null)
                        continue;
                    RemoveAnimatorMachineTransition(subMachine);
                }
            }
            AssetDatabase.SaveAssets();
        }

        private static void RemoveAnimatorMachineTransition(AnimatorStateMachine machine) {
            ChildAnimatorState[] states = machine.states;
            for (int stateIndex = 0; stateIndex < states.Length; stateIndex++) {
                AnimatorState state = states[stateIndex].state;
                if (state == null || state.transitions == null)
                    continue;
                int transIndex = state.transitions.Length - 1;
                for (; transIndex >= 0; transIndex--)
                    state.RemoveTransition(state.transitions[transIndex]);
            }
        }

        public static void SetAnimationClipData(int index) {
            AnimationModel.SetCurrentAnimationClip(index);
        }

        public static void Play() {
            m_lastTime = EditorApplication.timeSinceStartup;
            EditorApplication.update += Update;
            if (m_isGenericClip)
                SkillAnimator.Play(AnimationModel.SelectAnimationClip);
            else
                SkillClip.Play(m_model, AnimationModel.SelectAnimationClip);
        }

        private static void Update() {
            if ((m_isGenericClip && SkillAnimator.IsPlayOver) || (!m_isGenericClip && SkillClip.IsPlayOver))
                Stop();
            double currentTime = EditorApplication.timeSinceStartup;
            float deltaTime = (float)(currentTime - m_lastTime);
            m_lastTime = currentTime;
            if (m_isGenericClip)
                SkillAnimator.Update(deltaTime);
            else
                SkillClip.Update(deltaTime);
        }

        public static void Stop() {
            EditorApplication.update -= Update;
        }

        public static void Reset() {
            if (m_isGenericClip && m_model != null) {
                string copyPath = Config.GetAnimatorControllerCopyPath(m_model.name);
                if (File.Exists(copyPath)) {
                    string sourcePath = Config.GetAnimatorControllerPath(m_model.name);
                    sourcePath = Tool.ProjectPathToFullPath(sourcePath);
                    File.Copy(copyPath, sourcePath, true);
                    File.Delete(copyPath);
                    AssetDatabase.SaveAssets();
                }
            }
            if (m_model) {
                Object.DestroyImmediate(m_model);
                m_model = null;
            }
        }

        public static void Exit() {
            Reset();
            EditorApplication.update = null;
            EditorWindow.CloseWindow();
            EditorScene.UnregisterSceneGUI();
        }
    }
}