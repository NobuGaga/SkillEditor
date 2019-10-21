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

        private static bool m_isPlaying;

        private static List<CubeData> m_listDrawCubeData = new List<CubeData>();

        public static void Start(string prefabPath) {
            Reset();
            m_model = LoadPrefab(prefabPath);
            if (File.Exists(Config.WeaponPath)) {
                GameObject weapon = LoadPrefab(Config.WeaponPath);
                Transform[] nodes = m_model.transform.GetComponentsInChildren<Transform>();
                Transform rightHand = null;
                if (nodes != null)
                    for (int index = 0; index < nodes.Length; index++)
                        if (nodes[index].name == "R_Weapon_Point")
                            rightHand = nodes[index];
                if (rightHand != null) {
                    weapon.transform.SetParent(rightHand);
                    weapon.transform.localPosition = Vector3.zero;
                }
            }
            Selection.activeGameObject = m_model;
            InitAnimation();
            InitAnimClipData();
            EditorScene.SetDrawCubeData(m_listDrawCubeData);
            EditorScene.RegisterSceneGUI();
            EditorWindow.InitData(AnimationModel.AnimationClipNames, AnimationModel.AnimationClipIndexs);
            EditorWindow.Open();
        }

        private static GameObject LoadPrefab(string path) {
            GameObject prefab = PrefabUtility.LoadPrefabContents(path);
            GameObject gameObject = Object.Instantiate(prefab);
            gameObject.name = prefab.name;
            PrefabUtility.UnloadPrefabContents(prefab);
            return gameObject;
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

        private static List<AnimationClip> m_listAnimationClip = new List<AnimationClip>(Config.ModelStateClipCount);
        private static AnimationClip[] GetAllAnimationClip() {
            string[] fileNames = Directory.GetFiles(Config.ClipGroupFullPath);
            m_listAnimationClip.Clear();
            for (int index = 0; index < fileNames.Length; index++) {
                if (fileNames[index].Contains(".meta") || !fileNames[index].Contains("@") ||
                    !(fileNames[index].Contains(".fbx") || fileNames[index].Contains(".FBX")))
                    continue;
                string path = Tool.FullPathToProjectPath(fileNames[index]);
                AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
                if (clip == null)
                    continue;
                m_listAnimationClip.Add(clip);
            }
            return m_listAnimationClip.ToArray();
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
            LuaAnimClipModel.SetCurrentEditClipName(AnimationModel.SelectAnimationClipName);
        }

        public static void SetAnimClipData(State state) {
            LuaAnimClipModel.ClipDataState = state;
        }

        public static void AddNewKeyFrameData(string key) {
            LuaAnimClipModel.AddNewKeyFrameData(key);
        }

        public static void SetAnimClipData(string key, int index, KeyFrameData data) {
            LuaAnimClipModel.SetKeyFrameData(key, index, data);
        }

        public static void Play() {
            AnimationClip selectAnimationClip = AnimationModel.SelectAnimationClip;
            if (selectAnimationClip == null)
                return;
            if (m_isPlaying)
                return;
            m_isPlaying = true;
            m_lastTime = EditorApplication.timeSinceStartup;
            EditorApplication.update += Update;
            if (m_isGenericClip)
                SkillAnimator.Play(selectAnimationClip);
            else
                SkillClip.Play(m_model, selectAnimationClip);
        }

        public static void Pause() {
            if (!m_isPlaying)
                return;
            if (m_isGenericClip)
                SkillAnimator.Pause();
            else
                SkillClip.Pause();
        }

        public static void Stop() {
            if (!m_isPlaying)
                return;
            m_isPlaying = false;
            EditorApplication.update -= Update;
            if (m_isGenericClip)
                SkillAnimator.Stop();
            else
                SkillClip.Stop();
            m_listDrawCubeData.Clear();
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
            m_listDrawCubeData.Clear();
            if (LuaAnimClipModel.ListCollision.Count == 0)
                return;
            float time;
            if (m_isGenericClip)
                time = SkillAnimator.PlayTime;
            else
                time = SkillClip.PlayTime;
            var list = LuaAnimClipModel.ListCollision;
            float minTime = list[0].Key - Config.FramesPerSecond;
            float maxTime = list[list.Count - 1].Key + Config.FramesPerSecond;
            if (time < minTime || time > maxTime)
                return;
            for (int index = 0; index < list.Count; index++)
                if (IsInCollisionTime(time, list[index].Key))
                    m_listDrawCubeData.Add(list[index].Value);
            EditorScene.SetDrawCubeData(m_listDrawCubeData);
        }

        private static bool IsInCollisionTime(float curTime, float collisionTime) {
            float minTime = collisionTime - Config.FramesPerSecond;
            float maxTime = collisionTime + Config.FramesPerSecond;
            return curTime >= minTime && curTime <= maxTime;
        }

        private static void WriteAnimClipData() {
            LuaWriter.Write();
        }

        public static void Reset() {
            LuaAnimClipModel.Reset();
            m_listDrawCubeData.Clear();
            m_isPlaying = false;
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