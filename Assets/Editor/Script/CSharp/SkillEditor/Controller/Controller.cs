﻿using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using Lua;
using AnimClipData = Lua.AnimClipData;
using EffectConf = Lua.EffectConf;

namespace SkillEditor {

    internal static class Controller {

        private static GameObject m_model;
        private static Transform m_footTransform;
        private static BaseAnimation m_modelAnimation;
        public static float PlayTime => m_modelAnimation != null ? m_modelAnimation.PlayTime : 0;

        private static GameObject m_rightWeapon;
        private static BaseAnimation m_rightWeaponAnimation;
        private static GameObject m_leftWeapon;
        private static BaseAnimation m_leftWeaponAnimation;
        private static bool m_isNoWeaponClip = false;

        private static Dictionary<uint, GameObject> m_dicIDEffectObject = new Dictionary<uint, GameObject>();
        private static Dictionary<uint, ParticleSystem[]> m_dicIDEffects = new Dictionary<uint, ParticleSystem[]>();
        private static Dictionary<uint, Dictionary<string, float>> m_dicIDObjectNameDelay = new Dictionary<uint, Dictionary<string, float>>();
        private static Dictionary<uint, SkillAnimator> m_dicIDEffectAnimation = new Dictionary<uint, SkillAnimator>();

        private static double m_playStartTime;
        private static double m_lastTime;

        private static Dictionary<float, Vector3> m_dicTimePosition = new Dictionary<float, Vector3>();
        private static List<KeyValuePair<Vector3, AnimClipData.ICubeData>> m_listPointHitData = new List<KeyValuePair<Vector3, AnimClipData.ICubeData>>();
        private static List<KeyValuePair<Vector3, AnimClipData.ICubeData>> m_listPointGrabData = new List<KeyValuePair<Vector3, AnimClipData.ICubeData>>();
        private static List<KeyValuePair<Vector3, AnimClipData.ICubeData>> m_listPointBlockData = new List<KeyValuePair<Vector3, AnimClipData.ICubeData>>();

        public static void Start(string prefabPath) {
            Reset();
            m_model = LoadPrefab(prefabPath);
            m_footTransform = Tool.FindContainChild(m_model.transform, Config.DrawCubeNodeName);
            Selection.activeGameObject = null;
            InitModelAnimation();
            InitLuaConfigData();
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
            Animator animator = m_model.GetComponent<Animator>();
            var controller = animator.runtimeAnimatorController;
            AnimatorControllerManager.RemoveAllAnimatorTransition(controller.name, sourcePath);
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
                if (fileNames[index].Contains(Config.MetaExtension) || !fileNames[index].Contains(Config.AnimationClipSymbol) ||
                    !(fileNames[index].Contains(Config.ClipLowerExtension) || fileNames[index].Contains(Config.ClipUpperExtension)))
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
            LuaReader.Read<EffectConf.EffectData>(true);
            LuaAnimClipModel.SetCurrentModelName(m_model.name);
            LuaEffectConfModel.Init();
            LuaAnimClipModel.SetEffectChangeCallback(SetEffectData);
        }

        public static void SetRightWeapon(int index) => SetWeapon(index, true);
        public static void SetLeftWeapon(int index) => SetWeapon(index, false);

        private static void SetWeapon(int index, bool isRight) {
            string parentNodeName = isRight ? Config.RightWeaponNode : Config.LeftWeaponNode;
            GameObject weapon = isRight ? m_rightWeapon : m_leftWeapon;
            string path = WeaponModel.GetWeaponPrefabPath(ModelDataModel.ModelName, index);
            if (!File.Exists(Tool.ProjectPathToFullPath(path)))
                return;
            Transform handTransform = Tool.GetTransformByName(m_model, parentNodeName);
            if (handTransform == null)
                return;
            if (weapon != null)
                Object.DestroyImmediate(weapon);
            weapon = LoadPrefab(path);
            weapon.transform.SetParent(handTransform);
            Tool.NormalizeTransform(weapon);
            if (isRight)
                m_rightWeapon = weapon;
            else
                m_leftWeapon = weapon;
            SetWeaponAnimation(isRight);
        }

        private static void SetWeaponAnimation(bool isRight) {
            GameObject weapon = isRight ? m_rightWeapon : m_leftWeapon;
            if (weapon == null) {
                if (isRight)
                    m_rightWeaponAnimation = null;
                else
                    m_leftWeaponAnimation = null;
                return;
            }
            BaseAnimation animation = null;
            if (WeaponModel.CheckModelHasWeaponClip(ModelDataModel.ModelName, weapon.name))
                SetAnimation(ref animation, WeaponModel.GetGenericState(ModelDataModel.ModelName, weapon.name), weapon);
            else
                animation = null;
            if (isRight)
                m_rightWeaponAnimation = animation;
            else
                m_leftWeaponAnimation = animation;
        }

        private static void SetEffectData() {
            foreach (var timeEffectsPair in LuaAnimClipModel.ListEffect)
                foreach (var effectData in timeEffectsPair.Value)
                    if (effectData.type != AnimClipData.EffectType.Hit && !m_dicIDEffectObject.ContainsKey(effectData.id))
                        CreateEffect(effectData); 
        }

        private static void CreateEffect(AnimClipData.EffectData animClipEffect) {
            EffectConf.EffectData data = LuaEffectConfModel.GetEffectData(animClipEffect.id);
            string path = Tool.GetAssetProjectPath(data.resourceName, Config.PrefabExtension, Config.SkillEffectPath);
            GameObject effectNode = LoadPrefab(Tool.ProjectPathToFullPath(path));
            SetEffectTransform(data, effectNode);
            SetEffectParticle(animClipEffect.id, effectNode);
            m_dicIDEffectObject.Add(animClipEffect.id, effectNode);
        }

        private static void SetEffectTransform(EffectConf.EffectData data, GameObject effectNode) {
            if (data.pivotType == EffectConf.PivotType.Follow) {
                Transform parent = null;
                if (data.parentPivotType == EffectConf.ParentPivotType.Body)
                    parent = m_model.transform.Find(data.pivotNodeName);
                else {
                    if (m_rightWeapon != null)
                        parent = m_rightWeapon.transform.Find(data.pivotNodeName);
                    if (parent == null && m_leftWeapon != null)
                        parent = m_leftWeapon.transform.Find(data.pivotNodeName);
                }
                if (parent)
                    effectNode.transform.SetParent(parent);    
            }
            Tool.NormalizeTransform(effectNode);
            if (!data.offset.IsNullTable())
                effectNode.transform.localPosition = data.offset.Vector;
            if (!data.scale.IsNullTable())
                effectNode.transform.localScale = data.scale.Vector;
            if (!data.rotation.IsNullTable())
                effectNode.transform.localRotation = Quaternion.Euler(data.rotation.Vector);
        }

        private static void SetEffectParticle(uint id, GameObject effectNode) {
            ParticleSystem[] particles = effectNode.GetComponentsInChildren<ParticleSystem>(true);
            if (particles == null || particles.Length == 0)
                return;
            m_dicIDEffects.Add(id, particles);
            for (ushort index = 0; index < particles.Length; index++) {
                ParticleSystem particle = particles[index];
                if (!FilterParticleObject(particle))
                    continue;
                SetUseAutoSeedEffect(effectNode, particle);
                string name = particle.name;
                float startDelay = particle.main.startDelayMultiplier;
                if (!m_dicIDObjectNameDelay.ContainsKey(id))
                    m_dicIDObjectNameDelay.Add(id, new Dictionary<string, float>());
                if (m_dicIDObjectNameDelay[id].ContainsKey(name))
                    m_dicIDObjectNameDelay[id][name] = startDelay;
                else
                    m_dicIDObjectNameDelay[id].Add(name, startDelay);
#pragma warning disable 0618
                particle.startDelay = 0;
#pragma warning restore 0618
            }
            Animator animator = effectNode.GetComponent<Animator>();
            if (animator == null)
                return;
            AnimatorState state = AnimatorControllerManager.GetAnimatorControllerFirstStateName(animator, Config.SkillEffectPath);
            if (state == null || state.motion == null)
                return;
            SkillAnimator animation = new SkillAnimator(animator);
            animation.Record(state);
            m_dicIDEffectAnimation.Add(id, animation);
        }

        private static bool FilterParticleObject(ParticleSystem particle) {
            string[] excluteComponents = Config.EffectExcluteComponents;
            if (excluteComponents == null || excluteComponents.Length == 0)
                return true;
            for (ushort index = 0; index < excluteComponents.Length; index++) {
                string componentName = excluteComponents[index];
                Component component = particle.GetComponent(componentName);
                if (component != null)
                    particle.gameObject.SetActive(false);
                if (!particle.gameObject.activeSelf)
                    break;
            }
            return particle.gameObject.activeSelf;
        }

        private static void SetUseAutoSeedEffect(GameObject rootNode, ParticleSystem particle) {
            var dic = Config.UseAutoSeedEffect;
            string rootNodeName = rootNode.name;
            if (dic.ContainsKey(rootNodeName) && dic[rootNodeName].ContainsKey(particle.name))
                particle.useAutoRandomSeed = true;
        }

        public static void ReloadEffectConf() {
            LuaEffectConfModel.Reset();
            LuaReader.Read<EffectConf.EffectData>(true);
            LuaEffectConfModel.Init();
            foreach (var idObjectPair in m_dicIDEffectObject) {
                uint id = idObjectPair.Key;
                GameObject effectNode = idObjectPair.Value;
                EffectConf.EffectData data = LuaEffectConfModel.GetEffectData(id);
                if (data.IsNullTable())
                    continue;
                SetEffectTransform(data, effectNode);
            }
        }

        #region Set Lua AnimClipData Config Data
        public static void SetAnimationClipData(int index) {
            Stop();
            ResetDrawHitData();
            Tool.NormalizeTransform(m_model);
            AnimationModel.SetCurrentAnimationClip(index);
            LuaAnimClipModel.SetCurrentClipName(AnimationModel.SelectAnimationClipName);
            AnimationClip selectAnimationClip = AnimationModel.SelectAnimationClip;
            if (selectAnimationClip == null) {
                m_isNoWeaponClip = true;
                m_rightWeaponAnimation = null;
                m_leftWeaponAnimation = null;
                return;
            }
            m_isNoWeaponClip = !WeaponModel.CheckModelHasClip(ModelDataModel.ModelName);
            SetWeaponAnimation(true);
            SetWeaponAnimation(false);
        }

        public static void SetAnimationStateData(AnimClipData.State state) => LuaAnimClipModel.SetCurrentState(state);
        
        public static void AddNewClipGroupData(uint id) {
            LuaAnimClipModel.AddNewClipGroupData(id);
            RefreshScene();
        }
        public static void DeleteClipGroupData() {
            LuaAnimClipModel.DeleteClipGroupData();
            RefreshScene();
        }
        public static void SetClipGroupID(uint id) {
            LuaAnimClipModel.SetClipGroupID(id);
            RefreshScene();
        }

        public static void AddFrameData() => LuaAnimClipModel.AddFrameData();
        public static void AddCopyFrameData(AnimClipData.FrameData data) => LuaAnimClipModel.AddCopyFrameData(data);
        public static void DeleteFrameData(int index) => LuaAnimClipModel.DeleteFrameData(index);
        public static void SetFrameDataTime(int index, float time) => LuaAnimClipModel.SetFrameDataTime(index, time);
        public static void SetFrameDataEndTime(int index, float endTime) => LuaAnimClipModel.SetFrameDataEndTime(index, endTime);

        public static void AddPriorityFrameData(int index, AnimClipData.FrameType frameType) => LuaAnimClipModel.AddPriorityFrameData(index, frameType);
        public static void DeletePriorityFrameData(int index, AnimClipData.FrameType frameType) => LuaAnimClipModel.DeletePriorityFrameData(index, frameType);
        public static void SetFramePriorityData(int index, AnimClipData.FrameType frameType, ushort priority) => LuaAnimClipModel.SetFramePriorityData(index, frameType, priority);
        public static void SetFrameLoopData(int index, AnimClipData.FrameType frameType, bool isLoop) => LuaAnimClipModel. SetFrameLoopData(index, frameType, isLoop);

        public static void AddUngrabFrameData(int index) => LuaAnimClipModel.AddUngrabFrameData(index);
        public static void SetUngrabFrameData(int index, AnimClipData.UngrabFrameData data) => LuaAnimClipModel.SetUngrabFrameData(index, data);

        public static void AddCameraFrameData(int index) => LuaAnimClipModel.AddCameraFrameData(index);
        public static void DeleteCameraFrameData(int index) => LuaAnimClipModel.DeleteCameraFrameData(index);
        public static void SetCameraFrameData(int index, AnimClipData.CameraFrameData data) => LuaAnimClipModel.SetCameraFrameData(index, data);

        public static void AddTrackChangeFrameData(int index) => LuaAnimClipModel.AddTrackChangeFrameData(index);
        public static void DeleteTrackChangeFrameData(int index) => LuaAnimClipModel.DeleteTrackChangeFrameData(index);
        public static void SetTrackChangeFrameData(int index, AnimClipData.TrackChangeFrameData data) => LuaAnimClipModel.SetTrackChangeFrameData(index, data);

        public static void AddIDFrameData(int index, AnimClipData.FrameType frameType) => LuaAnimClipModel.AddIDFrameData(index, frameType);
        public static void DeleteIDFrameData(int index, AnimClipData.FrameType frameType) => LuaAnimClipModel.DeleteIDFrameData(index, frameType);
        public static void SetIDFrameData(int index, AnimClipData.FrameType frameType, AnimClipData.IDFrameData data) => LuaAnimClipModel.SetIDFrameData(index, frameType, data);

        public static void AddNewCustomData(int index, AnimClipData.FrameType frameType) => LuaAnimClipModel.AddNewCustomSubData(index, frameType);
        public static void DeleteCustomData(int frameIndex, int deleteIndex, AnimClipData.FrameType frameType) => LuaAnimClipModel.DeleteCustomSubData(frameIndex, deleteIndex, frameType);
        public static void SetCustomeSubData(int frameIndex, ITable data, AnimClipData.FrameType frameType) => LuaAnimClipModel.SetCustomeSubData(frameIndex, data, frameType);
        #endregion

        public static void Play() {
            if (m_modelAnimation.IsPlaying)
                return;
            StopEffect();
            AnimationClip selectAnimationClip = AnimationModel.SelectAnimationClip;
            if (selectAnimationClip == null)
                return;
            m_lastTime = EditorApplication.timeSinceStartup;
            m_playStartTime = m_lastTime;
            EditorApplication.update += Update;
            m_modelAnimation.Play(selectAnimationClip);
            if (m_isNoWeaponClip)
                return;
            if (m_rightWeaponAnimation != null) {
                AnimationClip clip = WeaponModel.GetAnimationClip(ModelDataModel.ModelName, m_rightWeapon.name, selectAnimationClip.name);
                m_rightWeaponAnimation.Play(clip);
            }
            if (m_leftWeaponAnimation != null) {
                AnimationClip clip = WeaponModel.GetAnimationClip(ModelDataModel.ModelName, m_leftWeapon.name, selectAnimationClip.name);
                m_leftWeaponAnimation.Play(clip);
            }
        }

        public static void Pause() {
            m_modelAnimation.Pause();
            if (m_isNoWeaponClip)
                return;
            if (m_rightWeaponAnimation != null)
                m_rightWeaponAnimation.Pause();
            if (m_leftWeaponAnimation != null)
                m_leftWeaponAnimation.Pause();
        }

        public static void Stop() {
            EditorApplication.update = null;
            m_modelAnimation.Stop();
            if (!m_isNoWeaponClip) {
                if (m_rightWeaponAnimation != null)
                    m_rightWeaponAnimation.Stop();
                if (m_leftWeaponAnimation != null)
                    m_leftWeaponAnimation.Stop();
            }
            StopEffect();
            SceneView.RepaintAll();
        }

        private static void StopEffect() {
            foreach (var idParticlesPair in m_dicIDEffects) {
                uint id = idParticlesPair.Key;
                if (!m_dicIDEffectObject[id].activeSelf)
                    continue;
                ParticleSystem[] particles = idParticlesPair.Value;
                for (ushort index = 0; index < particles.Length; index++)
                    particles[index].Simulate(0);
            }
            foreach (var idEffectAnimation in m_dicIDEffectAnimation) {
                SkillAnimator animation = idEffectAnimation.Value;
                animation.Stop();
            }
            foreach (var idObjectPair in m_dicIDEffectObject)
                if (idObjectPair.Value.activeSelf)
                    idObjectPair.Value.SetActive(false);
        }

        public static void SetAnimationPlayTime(float time) {
            if (m_modelAnimation.IsPlaying)
                return;
            AnimationClip selectAnimationClip = AnimationModel.SelectAnimationClip;
            m_modelAnimation.SetAnimationPlayTime(selectAnimationClip, time);
            if (!m_isNoWeaponClip) {
                if (m_rightWeaponAnimation != null)
                    m_rightWeaponAnimation.SetAnimationPlayTime(selectAnimationClip, time);
                if (m_leftWeaponAnimation != null)
                    m_leftWeaponAnimation.SetAnimationPlayTime(selectAnimationClip, time);
            }
            if (time <= 0)
                StopEffect();
            else
                SetPlayEffectTime(time);            
            SetDrawCubeData(time);
        }

        private static void Update() {
            double currentTime = EditorApplication.timeSinceStartup;
            float deltaTime = (float)(currentTime - m_lastTime);
            m_lastTime = currentTime;
            m_modelAnimation.Update(deltaTime);
            if (!m_isNoWeaponClip) {
                if (m_rightWeaponAnimation != null)
                    m_rightWeaponAnimation.Update(deltaTime);
                if (m_leftWeaponAnimation != null)
                    m_leftWeaponAnimation.Update(deltaTime);
            }
            EditorWindow.RefreshRepaint();
            SetPlayEffectTime((float)(currentTime - m_playStartTime));
            SetDrawCubeData(deltaTime);
            if (m_modelAnimation.IsPlayOver) {
                StopEffect();
                EditorApplication.update = null;
            }
        }

        private static void SetPlayEffectTime(float sampleTime) {
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
                    if (m_dicIDEffectObject.ContainsKey(data.id) && !m_dicIDEffectObject[data.id].activeSelf)
                        m_dicIDEffectObject[data.id].SetActive(true);
                    if (m_dicIDEffects.ContainsKey(data.id) && m_dicIDObjectNameDelay.ContainsKey(data.id)) {
                        Dictionary<string, float> dicParticleNameTime = m_dicIDObjectNameDelay[data.id];
                        ParticleSystem[] particles = m_dicIDEffects[data.id];
                        for (ushort particleIndex = 0; particleIndex < particles.Length; particleIndex++) {
                            ParticleSystem particle = particles[particleIndex];
                            if (!dicParticleNameTime.ContainsKey(particle.name))
                                continue;
                            float simulateTime = time + dicParticleNameTime[particle.name] - Config.RuntimeEffectDelay;
                            if (sampleTime >= simulateTime)
                                particle.Simulate(sampleTime - simulateTime);
                            else
                                particle.Simulate(0);
                        }
                    }
                    if (m_dicIDEffectAnimation.ContainsKey(data.id)) {
                        SkillAnimator animation = m_dicIDEffectAnimation[data.id];
                        animation.SetStateAnimationPlayTime(sampleTime - time);
                    }
                }
            }
        }

        private static void SetDrawCubeData(float animationTime) {
            SetDrawHitData(animationTime);
            SetDrawGrabData(animationTime);
            SetDrawBlockData(animationTime);
        }

        private static void SetDrawHitData(float animationTime) {
            if (LuaAnimClipModel.ListCollision.Count == 0)
                return;
            m_listPointHitData.Clear();
            float time = m_modelAnimation.PlayTime;
            List<KeyValuePair<float, AnimClipData.HitData[]>> list = LuaAnimClipModel.ListCollision;
            if (IsOutOfTimeArea(list))
                return;
            for (int index = 0; index < list.Count; index++) {
                float triggerTime = list[index].Key;
                if (!IsInCollisionTime(time, triggerTime))
                    continue;
                AddTriggerTimePosition(triggerTime, animationTime);
                foreach (AnimClipData.HitData data in list[index].Value)
                    m_listPointHitData.Add(new KeyValuePair<Vector3, AnimClipData.ICubeData>(m_dicTimePosition[triggerTime], data));
            }
            EditorScene.SetDrawHitData(m_listPointHitData);
        }

        private static void SetDrawGrabData(float animationTime) {
            if (LuaAnimClipModel.ListGrabCollision.Count == 0)
                return;
            m_listPointGrabData.Clear();
            float time = m_modelAnimation.PlayTime;
            List<KeyValuePair<float, AnimClipData.GrabData[]>> list = LuaAnimClipModel.ListGrabCollision;
            if (IsOutOfTimeArea(list))
                return;
            for (int index = 0; index < list.Count; index++) {
                float triggerTime = list[index].Key;
                if (!IsInCollisionTime(time, triggerTime))
                    continue;
                AddTriggerTimePosition(triggerTime, animationTime);
                foreach (AnimClipData.GrabData data in list[index].Value)
                    m_listPointGrabData.Add(new KeyValuePair<Vector3, AnimClipData.ICubeData>(m_dicTimePosition[triggerTime], data));
            }
            EditorScene.SetDrawGrabData(m_listPointGrabData);
        }

        private static void SetDrawBlockData(float animationTime) {
            if (LuaAnimClipModel.ListBlockCollision.Count == 0)
                return;
            m_listPointBlockData.Clear();
            float time = m_modelAnimation.PlayTime;
            List<KeyValuePair<float, AnimClipData.BlockData[]>> list = LuaAnimClipModel.ListBlockCollision;
            if (IsOutOfTimeArea(list))
                return;
            for (int index = 0; index < list.Count; index++) {
                float triggerTime = list[index].Key;
                if (!IsInCollisionTime(time, triggerTime))
                    continue;
                AddTriggerTimePosition(triggerTime, animationTime);
                foreach (AnimClipData.BlockData data in list[index].Value)
                    m_listPointBlockData.Add(new KeyValuePair<Vector3, AnimClipData.ICubeData>(m_dicTimePosition[triggerTime], data));
            }
            EditorScene.SetDrawBlockData(m_listPointBlockData);
        }

        private static bool IsOutOfTimeArea<T>(List<KeyValuePair<float, T>> list) {
            float time = m_modelAnimation.PlayTime;
            float minTime = list[0].Key;
            float maxTime = list[list.Count - 1].Key + Config.DrawCubeLastTime;
            return time < minTime || time > maxTime;
        }

        private static void AddTriggerTimePosition(float triggerTime, float animationTime) {
            if (m_dicTimePosition.ContainsKey(triggerTime))
                return;
            Vector3 position = m_footTransform.position;
            if (!Config.IsNoRuntimeCubeDelay) {
                m_modelAnimation.SetAnimationPlayTime(AnimationModel.SelectAnimationClip, triggerTime + Config.RuntimeCubeDelay);
                position = m_footTransform.position;
                m_modelAnimation.SetAnimationPlayTime(AnimationModel.SelectAnimationClip, animationTime);
            }
            m_dicTimePosition.Add(triggerTime, position);
        }

        private static bool IsInCollisionTime(float curTime, float collisionTime) {
            float minTime = collisionTime;
            float maxTime = collisionTime + Config.DrawCubeLastTime;
            return curTime >= minTime && curTime <= maxTime;
        }

        public static void WriteAnimClipData() {
            LuaWriter.Write<AnimClipData.AnimClipData>();
            Shell.Run("CopyAnimClipFile");
        }

        private static void RefreshScene() {
            Stop();
            ResetDrawHitData();
        }

        private static void ResetDrawHitData() {
            m_dicTimePosition.Clear();
            m_listPointHitData.Clear();
            m_listPointGrabData.Clear();
        }

        public static void Reset() {
            LuaAnimClipModel.Reset();
            LuaEffectConfModel.Reset();
            ResetDrawHitData();
            if (m_model != null) {
                Object.DestroyImmediate(m_model);
                m_model = null;
            }
            if (m_rightWeapon != null) {
                Object.DestroyImmediate(m_rightWeapon);
                m_rightWeapon = null;
            }
            if (m_leftWeapon != null) {
                Object.DestroyImmediate(m_leftWeapon);
                m_leftWeapon = null;
            }
            m_modelAnimation = null;
            m_rightWeaponAnimation = null;
            m_leftWeaponAnimation = null;
            m_dicIDEffectObject.Clear();
            m_dicIDEffects.Clear();
            m_dicIDObjectNameDelay.Clear();
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