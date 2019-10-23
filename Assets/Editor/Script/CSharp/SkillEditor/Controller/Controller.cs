using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using SkillEditor.LuaStructure;

namespace SkillEditor {

    internal static class Controller {

        private static GameObject m_model;
        private static BaseAnimation m_modelAnimation;
        private static GameObject m_weapon;

        private static bool m_isGenericClip;

        private static double m_lastTime;

        private static bool m_isPlaying;

        private static List<CubeData> m_listDrawCubeData = new List<CubeData>();

        public static void Start(string prefabPath) {
            Reset();
            m_model = LoadPrefab(prefabPath);
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
            if (!m_isGenericClip) {
                if (m_modelAnimation is SkillClip)
                    m_modelAnimation.Init(m_model);
                else
                    m_modelAnimation = new SkillClip(m_model);
                return;
            }
            Animator animator = m_model.GetComponent<Animator>();
            if (animator == null)
                Debug.LogError("Prefab's animator is not exit");
            if (m_modelAnimation is SkillAnimator)
                m_modelAnimation.Init(animator);
            else
                m_modelAnimation = new SkillAnimator(animator);
            string sourcePath = Tool.FullPathToProjectPath(Config.ControllerPath);
            AnimatorControllerManager.RemoveAllAnimatorTransition(m_model.name, sourcePath);
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

        private static void InitAnimClipData() {
            LuaReader.Read<AnimClipData>();
            LuaAnimClipModel.SetCurrentEditModelName(m_model.name);
        }

        public static void SetWeapon(int index) {
            string path = WeaponModel.GetWeaponPrefabPath("nvwang", index);
            if (!File.Exists(Tool.ProjectPathToFullPath(path)))
                return;
            Transform[] nodes = m_model.transform.GetComponentsInChildren<Transform>();
            Transform rightHand = null;
            if (nodes != null)
                for (int i = 0; i < nodes.Length; i++)
                    if (nodes[i].name == "R_Weapon_Point")
                        rightHand = nodes[i];
            if (rightHand != null) {
                GameObject weapon = LoadPrefab(path);
                weapon.transform.SetParent(rightHand);
                weapon.transform.localPosition = Vector3.zero;
            }
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

        public static void AddNewEffectData(int index) {
            LuaAnimClipModel.AddNewEffectData(index);
        }

        public static void AddNewCubeData(int index) {
            LuaAnimClipModel.AddNewCubeData(index);
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
                m_modelAnimation.Play(selectAnimationClip);
        }

        public static void Pause() {
            if (!m_isPlaying)
                return;
            m_modelAnimation.Pause();
        }

        public static void Stop() {
            if (!m_isPlaying)
                return;
            m_isPlaying = false;
            EditorApplication.update -= Update;
            m_modelAnimation.Stop();
            m_listDrawCubeData.Clear();
        }

        private static void Update() {
            if (m_modelAnimation.IsPlayOver)
                Stop();
            double currentTime = EditorApplication.timeSinceStartup;
            float deltaTime = (float)(currentTime - m_lastTime);
            m_lastTime = currentTime;
            m_modelAnimation.Update(deltaTime);
            m_listDrawCubeData.Clear();
            if (LuaAnimClipModel.ListCollision.Count == 0)
                return;
            float time = m_modelAnimation.PlayTime;
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
            if (m_model) {
                Object.DestroyImmediate(m_model);
                m_model = null;
            }
            if (m_weapon) {
                Object.DestroyImmediate(m_weapon);
                m_weapon = null;
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