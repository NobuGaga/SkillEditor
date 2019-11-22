using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using Lua;
using Lua.AnimClipData;

namespace SkillEditor {

    internal static class Controller {

        private static GameObject m_model;
        private static BaseAnimation m_modelAnimation;
        public static float PlayTime => m_modelAnimation != null ? m_modelAnimation.PlayTime : 0;
        private static GameObject m_weapon;
        private static BaseAnimation m_weaponAnimation;
        private static bool m_isNoWeaponClip = false;

        private static double m_lastTime;

        private static List<CubeData> m_listDrawCubeData = new List<CubeData>();

        public static void Start(string prefabPath) {
            Reset();
            m_model = LoadPrefab(prefabPath);
            Selection.activeGameObject = null;
            InitModelAnimation();
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

        private static void InitModelAnimation() {
            AnimationModel.AnimationClips = GetAllAnimationClip();
            bool isGeneric = AnimationModel.GenericState();
            SetAnimation(ref m_modelAnimation, isGeneric, m_model);
            if (!isGeneric)
                return;
            string sourcePath = Tool.FullPathToProjectPath(Config.ControllerPath);
            AnimatorControllerManager.RemoveAllAnimatorTransition(m_model.name, sourcePath);
        }

        private static void SetAnimation(ref BaseAnimation animation, bool isGeneric, GameObject gameObject) {
            if (isGeneric) {
                Animator animator = gameObject.GetComponent<Animator>();
                if (animator == null)
                    Debug.LogError("Prefab's animator is not exit, prefab name " + gameObject.name);
                if (animation is SkillAnimator)
                    animation.Init(animator);
                else
                    animation = new SkillAnimator(animator);  
            }
            else {
                if (animation is SkillClip)
                    animation.Init(gameObject);
                else
                    animation = new SkillClip(gameObject);
            }
        }

        private static List<AnimationClip> m_listAnimationClip = new List<AnimationClip>(16);
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
            LuaReader.Read<Lua.EffectConf.EffectData>();
            LuaAnimClipModel.SetCurrentEditModelName(m_model.name);
            LuaEffectConfModel.Init();
        }

        public static void SetWeapon(int index) {
            string path = WeaponModel.GetWeaponPrefabPath(Config.TempModelName, index);
            if (!File.Exists(Tool.ProjectPathToFullPath(path)))
                return;
            Transform[] nodes = m_model.transform.GetComponentsInChildren<Transform>();
            if (nodes == null)
                return;
            Transform rightHand = null;
            for (int i = 0; i < nodes.Length; i++)
                if (nodes[i].name == "R_Weapon_Point") {
                    rightHand = nodes[i];
                    break;
                }
            if (rightHand == null)
                return;
            if (m_weapon != null)
                Object.DestroyImmediate(m_weapon);
            m_weapon = LoadPrefab(path);
            m_weapon.transform.SetParent(rightHand);
            m_weapon.transform.localPosition = Vector3.zero;
            if (WeaponModel.CheckModelHasWeaponClip(Config.TempModelName))
                SetAnimation(ref m_weaponAnimation, WeaponModel.GetGenericState(Config.TempModelName), m_weapon);
            else
                m_weaponAnimation = null;
        }

        public static void SetAnimationClipData(int index) {
            AnimationModel.SetCurrentAnimationClip(index);
            LuaAnimClipModel.SetCurrentEditClipName(AnimationModel.SelectAnimationClipName);
        }

        public static void SetAnimationStateData(State state) => LuaAnimClipModel.ClipDataState = state;

        public static void AddFrameData() => LuaAnimClipModel.AddFrameData();
        public static void DeleteFrameData(int index) => LuaAnimClipModel.DeleteFrameData(index);
        public static void SetFrameDataTime(int index, float time) => LuaAnimClipModel.SetFrameDataTime(index, time);

        public static void AddPriorityFrameData(int index, FrameType frameType) => LuaAnimClipModel.AddPriorityFrameData(index, frameType);
        public static void SetFramePriorityData(int index, FrameType frameType, ushort priority) => LuaAnimClipModel.SetFramePriorityData(index, frameType, priority);

        public static void AddNewCustomData(int index, FrameType frameType) => LuaAnimClipModel.AddNewCustomSubData(index, frameType);
        public static void DeleteCustomData(int frameIndex, int deleteIndex, FrameType frameType) => LuaAnimClipModel.DeleteCustomSubData(frameIndex, deleteIndex, frameType);
        public static void SetCustomeSubData(int frameIndex, ITable data, FrameType frameType) => LuaAnimClipModel.SetCustomeSubData(frameIndex, data, frameType);

        public static void Play() {
            if (m_modelAnimation.IsPlaying)
                return;
            AnimationClip selectAnimationClip = AnimationModel.SelectAnimationClip;
            if (selectAnimationClip == null)
                return;
            m_lastTime = EditorApplication.timeSinceStartup;
            EditorApplication.update += Update;
            m_modelAnimation.Play(selectAnimationClip);
            if (m_weaponAnimation == null)
                return;
            AnimationClip clip = WeaponModel.GetAnimationClip(Config.TempModelName, selectAnimationClip.name);
            m_isNoWeaponClip = clip == null;
            if (m_isNoWeaponClip)
                return;
            m_weaponAnimation.Play(clip);
        }

        public static void Pause() {
            m_modelAnimation.Pause();
            if (m_weaponAnimation != null && !m_isNoWeaponClip)
                m_weaponAnimation.Pause();
        }

        public static void Stop() {
            EditorApplication.update = null;
            m_modelAnimation.Stop();
            if (m_weaponAnimation != null && !m_isNoWeaponClip)
                m_weaponAnimation.Stop();
        }

        public static void SetAnimationPlayTime(float time) {
            if (m_modelAnimation.IsPlaying)
                return;
            AnimationClip selectAnimationClip = AnimationModel.SelectAnimationClip;
            m_modelAnimation.SetAnimationPlayTime(selectAnimationClip, time);
            if (m_weaponAnimation != null && !m_isNoWeaponClip)
                m_weaponAnimation.SetAnimationPlayTime(selectAnimationClip, time);
            SetDrawCubeData();
        }

        private static void Update() {
            double currentTime = EditorApplication.timeSinceStartup;
            float deltaTime = (float)(currentTime - m_lastTime);
            m_lastTime = currentTime;
            m_modelAnimation.Update(deltaTime);
            if (m_weaponAnimation != null && !m_isNoWeaponClip)
                m_weaponAnimation.Update(deltaTime);
            EditorWindow.RefreshRepaint();
            SetDrawCubeData();
            if (m_modelAnimation.IsPlayOver)
                EditorApplication.update = null;
        }

        private static void SetDrawCubeData() {
            m_listDrawCubeData.Clear();
            if (LuaAnimClipModel.ListCollision.Count == 0)
                return;
            float time = m_modelAnimation.PlayTime;
            var list = LuaAnimClipModel.ListCollision;
            float minTime = list[0].Key;
            float maxTime = list[list.Count - 1].Key + Config.FramesPerSecond;
            if (time < minTime || time > maxTime)
                return;
            for (int index = 0; index < list.Count; index++) {
                if (!IsInCollisionTime(time, list[index].Key))
                    continue;
                var array = list[index].Value;
                foreach (var data in array)
                    m_listDrawCubeData.Add(data);
            }
            EditorScene.SetDrawCubeData(m_listDrawCubeData);
        }

        private static bool IsInCollisionTime(float curTime, float collisionTime) {
            float minTime = collisionTime;
            float maxTime = collisionTime + Config.FramesPerSecond;
            return curTime >= minTime && curTime <= maxTime;
        }

        public static void WriteAnimClipData() {
            LuaWriter.Write<AnimClipData>();
        }

        public static void Reset() {
            LuaAnimClipModel.Reset();
            LuaEffectConfModel.Reset();
            m_listDrawCubeData.Clear();
            if (m_model != null) {
                Object.DestroyImmediate(m_model);
                m_model = null;
            }
            if (m_weapon != null) {
                Object.DestroyImmediate(m_weapon);
                m_weapon = null;
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