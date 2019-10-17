using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using SkillEditor.LuaStructure;

namespace SkillEditor {

    internal static class Controller {

        private static GameObject m_model;

        private static bool m_isGenericClip;

        private static double m_lastTime;

        public static void Start(string prefabPath) {
            Reset();
            GameObject prefab = PrefabUtility.LoadPrefabContents(prefabPath);
            m_model = Object.Instantiate(prefab);
            m_model.name = prefab.name;
            Selection.activeGameObject = m_model;
            PrefabUtility.UnloadPrefabContents(prefab);
            InitAnimation();
            InitAnimClipData();
            EditorScene.RegisterSceneGUI();
            EditorWindow.InitData(AnimationModel.AnimationClipNames, AnimationModel.AnimationClipIndexs);
            EditorWindow.Open();
        }

        private static void InitAnimation() {
            AnimationModel.AnimationClips = GetAllAnimationClip();
            m_isGenericClip = AnimationModel.GenericState();
            if (!m_isGenericClip)
                return;
            Animator animator = m_model.GetComponent<Animator>();
            if (animator == null)
                Debug.LogError("Prefab's animator is not exit");
            SkillAnimator.Animator = animator;
            RemoveAllAnimatorTransition();
        }

        private static AnimationClip[] GetAllAnimationClip() {
            string[] fileNames = Directory.GetFiles(Config.ClipGroupFullPath);
            List<AnimationClip> list = new List<AnimationClip>(Config.ModelStateClipCount);
            for (int index = 0; index < fileNames.Length; index++) {
                if (fileNames[index].Contains(".meta") || !fileNames[index].Contains("@") ||
                    !(fileNames[index].Contains(".fbx") || fileNames[index].Contains(".FBX")))
                    continue;
                string path = Tool.FullPathToProjectPath(fileNames[index]);
                AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
                if (clip == null)
                    continue;
                list.Add(clip);
            }
            return list.ToArray();
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

        private static void InitAnimClipData() {
            LuaReader.Read<AnimClipData>();
            LuaAnimClipModel.SetCurrentEditModelName(m_model.name);
        }

        public static void SetAnimationClipData(int index) {
            AnimationModel.SetCurrentAnimationClip(index);
        }

        private static void WriteAnimClipData() {
            LuaWriter.Write();
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
            LuaAnimClipModel.Reset();
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
            WriteAnimClipData();
            Reset();
            EditorApplication.update = null;
            EditorWindow.CloseWindow();
            EditorScene.UnregisterSceneGUI();
        }
    }
}