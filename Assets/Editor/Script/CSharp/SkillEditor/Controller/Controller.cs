using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using Lua;
using AnimClipData = Lua.AnimClipData;
using EffectConf = Lua.EffectConf;

namespace SkillEditor {

    internal static class Controller {

        private static GameObject m_model;
        private static BaseAnimation m_modelAnimation;
        public static float PlayTime => m_modelAnimation != null ? m_modelAnimation.PlayTime : 0;

        private static GameObject m_weapon;
        private static BaseAnimation m_weaponAnimation;
        private static bool m_isNoWeaponClip = false;

        private static Dictionary<int, ParticleSystem[]> m_dicIDEffects = new Dictionary<int, ParticleSystem[]>();
        private static Dictionary<int, BaseAnimation> m_dicIDEffectAnimation = new Dictionary<int, BaseAnimation>();

        private static double m_playStartTime;
        private static double m_lastTime;

        private static List<AnimClipData.CubeData> m_listDrawCubeData = new List<AnimClipData.CubeData>();

        public static void Start(string prefabPath) {
            Reset();
            m_model = LoadPrefab(prefabPath);
            Selection.activeGameObject = null;
            InitModelAnimation();
            InitLuaConfigData();
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
            string sourcePath = Tool.FullPathToProjectPath(ModelDataModel.ControllerPath);
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
            string[] fileNames = Directory.GetFiles(ModelDataModel.ClipFullPath);
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

        private static void InitLuaConfigData() {
            LuaReader.Read<AnimClipData.AnimClipData>();
            LuaReader.Read<EffectConf.EffectData>();
            LuaAnimClipModel.SetCurrentModelName(m_model.name);
            LuaEffectConfModel.Init();
            LuaAnimClipModel.SetEffectChangeCallback(SetEffectData);
        }

        public static void SetWeapon(int index) {
            string path = WeaponModel.GetWeaponPrefabPath(ModelDataModel.ModelName, index);
            if (!File.Exists(Tool.ProjectPathToFullPath(path)))
                return;
            Transform rightHand = Tool.GetTransformByName(m_model, Config.WeaponParentNode);
            if (rightHand == null)
                return;
            if (m_weapon != null)
                Object.DestroyImmediate(m_weapon);
            m_weapon = LoadPrefab(path);
            m_weapon.transform.SetParent(rightHand);
            Tool.NormalizeTransform(m_weapon);
            if (WeaponModel.CheckModelHasWeaponClip(ModelDataModel.ModelName))
                SetAnimation(ref m_weaponAnimation, WeaponModel.GetGenericState(ModelDataModel.ModelName), m_weapon);
            else
                m_weaponAnimation = null;
        }

        private static void SetEffectData() {
            foreach (var idEffectsPair in m_dicIDEffects)
                for (ushort index = 0; index < idEffectsPair.Value.Length; index++) {
                    ParticleSystem particle = idEffectsPair.Value[index];
                    particle.Stop();
                }
            // TODO Stop Animation
            // foreach (var idEffectAnimation in m_dicIDEffectAnimation) {
            //     BaseAnimation animation = idEffectAnimation.Value;
            //     animation.Stop();
            // }
            foreach (var timeEffectsPair in LuaAnimClipModel.ListEffect)
                foreach (var effectData in timeEffectsPair.Value) {
                    if (effectData.type == AnimClipData.EffectType.Hit)
                        continue;
                    bool isHasParticle = m_dicIDEffects.ContainsKey(effectData.id);
                    bool isHasAnimator = m_dicIDEffectAnimation.ContainsKey(effectData.id);
                    if (isHasParticle || isHasAnimator)
                        continue;
                    CreateEffect(effectData);
                }
        }

        private static void CreateEffect(AnimClipData.EffectData animClipEffect) {
            EffectConf.EffectData data = LuaEffectConfModel.GetEffectData((uint)animClipEffect.id);
            Transform parent = null;
            switch (data.parentPivotType) {
                case EffectConf.ParentPivotType.Body:
                    parent = Tool.GetTransformByName(m_model, data.pivotNodeName);
                    break;
                case EffectConf.ParentPivotType.Weapon:
                    if (m_weapon == null)
                        return;
                    parent = Tool.GetTransformByName(m_weapon, data.pivotNodeName);
                    break;
            }
            if (parent == null)
                return;
            string[] guids = AssetDatabase.FindAssets(data.resourceName, Config.ModelSkillEffectPath);
            if (guids == null || guids.Length == 0)
                return;
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            GameObject effectNode = LoadPrefab(Tool.ProjectPathToFullPath(path));
            effectNode.transform.SetParent(parent);
            Tool.NormalizeTransform(effectNode);
            Vector3 rotation = animClipEffect.rotation.Rotation;
            if (rotation != Vector3.zero)
                effectNode.transform.localRotation = Quaternion.Euler(rotation);
            SetEffectParticle(animClipEffect.id, effectNode);
        }

        private static void SetEffectParticle(int id, GameObject effectNode) {
            ParticleSystem[] particles = effectNode.GetComponentsInChildren<ParticleSystem>(true);
            if (particles == null || particles.Length == 0)
                return;
            m_dicIDEffects.Add(id, particles);
            Animator animator = effectNode.GetComponent<Animator>();
            if (animator == null)
                return;
            BaseAnimation animation = null;
            SetAnimation(ref animation, true, effectNode);
            m_dicIDEffectAnimation.Add(id, animation);
        }

        #region Set Lua AnimClipData Config Data
        public static void SetAnimationClipData(int index) {
            AnimationModel.SetCurrentAnimationClip(index);
            LuaAnimClipModel.SetCurrentClipName(AnimationModel.SelectAnimationClipName);
            SetEffectData();
        }

        public static void SetAnimationStateData(AnimClipData.State state) => LuaAnimClipModel.SetCurrentState(state);

        public static void SetAnimationClipID(uint id) => LuaAnimClipModel.SetCurrentClipID(id);
        public static void AddNewClipGroupData() => LuaAnimClipModel.AddNewClipGroupData();
        public static void DeleteClipGroupData() => LuaAnimClipModel.DeleteClipGroupData();

        public static void AddFrameData() => LuaAnimClipModel.AddFrameData();
        public static void DeleteFrameData(int index) => LuaAnimClipModel.DeleteFrameData(index);
        public static void SetFrameDataTime(int index, float time) => LuaAnimClipModel.SetFrameDataTime(index, time);

        public static void AddPriorityFrameData(int index, AnimClipData.FrameType frameType) => LuaAnimClipModel.AddPriorityFrameData(index, frameType);
        public static void DeletePriorityFrameData(int index, AnimClipData.FrameType frameType) => LuaAnimClipModel.DeletePriorityFrameData(index, frameType);
        public static void SetFramePriorityData(int index, AnimClipData.FrameType frameType, ushort priority) => LuaAnimClipModel.SetFramePriorityData(index, frameType, priority);

        public static void AddNewCustomData(int index, AnimClipData.FrameType frameType) => LuaAnimClipModel.AddNewCustomSubData(index, frameType);
        public static void DeleteCustomData(int frameIndex, int deleteIndex, AnimClipData.FrameType frameType) => LuaAnimClipModel.DeleteCustomSubData(frameIndex, deleteIndex, frameType);
        public static void SetCustomeSubData(int frameIndex, ITable data, AnimClipData.FrameType frameType) => LuaAnimClipModel.SetCustomeSubData(frameIndex, data, frameType);
        #endregion

        public static void Play() {
            if (m_modelAnimation.IsPlaying)
                return;
            AnimationClip selectAnimationClip = AnimationModel.SelectAnimationClip;
            if (selectAnimationClip == null)
                return;
            m_lastTime = EditorApplication.timeSinceStartup;
            m_playStartTime = m_lastTime;
            EditorApplication.update += Update;
            m_modelAnimation.Play(selectAnimationClip);
            if (m_weaponAnimation == null)
                return;
            AnimationClip clip = WeaponModel.GetAnimationClip(ModelDataModel.ModelName, selectAnimationClip.name);
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
            SetPlayEffectData(time);            
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
            SetPlayEffectData((float)(currentTime - m_playStartTime));
            SetDrawCubeData();
            if (m_modelAnimation.IsPlayOver)
                EditorApplication.update = null;
        }

        private static void SetPlayEffectData(float sampleTime) {
            var listEffect = LuaAnimClipModel.ListEffect;
            for (ushort pairIndex = 0; pairIndex < listEffect.Count; pairIndex++) {
                float time = listEffect[pairIndex].Key;
                if (sampleTime < time)
                    break;
                AnimClipData.EffectData[] datas = listEffect[pairIndex].Value;
                for (ushort dataIndex = 0; dataIndex < datas.Length; dataIndex++) {
                    AnimClipData.EffectData data = datas[dataIndex];
                    if (data.type == AnimClipData.EffectType.Hit)
                        continue;
                    if (m_dicIDEffectAnimation.ContainsKey(data.id)) {
                        // TODO
                    } else {
                        ParticleSystem[] particles = m_dicIDEffects[data.id];
                        for (ushort particleIndex = 0; particleIndex < particles.Length; particleIndex++) {
                            ParticleSystem particle = particles[particleIndex];
                            particle.Simulate(sampleTime - time);
                            particle.Play();
                            particle.Pause();
                        }
                    }
                }
            }
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

        public static void WriteAnimClipData() => LuaWriter.Write<AnimClipData.AnimClipData>();

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
            m_modelAnimation = null;
            m_weaponAnimation = null;
            m_dicIDEffects.Clear();
            m_dicIDEffectAnimation.Clear();
        }

        public static void Exit() {
            Reset();
            EditorApplication.update = null;
            EditorWindow.CloseWindow();
            EditorScene.UnregisterSceneGUI();
        }
    }
}