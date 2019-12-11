using UnityEditor;
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
        private static BaseAnimation m_modelAnimation;
        public static float PlayTime => m_modelAnimation != null ? m_modelAnimation.PlayTime : 0;

        private static GameObject m_weapon;
        private static BaseAnimation m_weaponAnimation;
        private static bool m_isNoWeaponClip = false;

        private static Dictionary<uint, GameObject> m_dicIDEffectObject = new Dictionary<uint, GameObject>();
        private static Dictionary<uint, ParticleSystem[]> m_dicIDEffects = new Dictionary<uint, ParticleSystem[]>();
        private static Dictionary<uint, Dictionary<string, float>> m_dicIDObjectNameDelay = new Dictionary<uint, Dictionary<string, float>>();
        private static Dictionary<uint, SkillAnimator> m_dicIDEffectAnimation = new Dictionary<uint, SkillAnimator>();

        private static double m_playStartTime;
        private static double m_lastTime;

        private static Dictionary<float, Vector3> m_dicTimePosition = new Dictionary<float, Vector3>();
        private static List<KeyValuePair<Vector3, AnimClipData.CubeData>> m_listPointCubeData = new List<KeyValuePair<Vector3, AnimClipData.CubeData>>();

        public static void Start(string prefabPath) {
            Reset();
            m_model = LoadPrefab(prefabPath);
            Selection.activeGameObject = null;
            InitModelAnimation();
            InitLuaConfigData();
            EditorScene.SetDrawCubeData(m_listPointCubeData);
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
            LuaReader.Read<EffectConf.EffectData>(true);
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
            foreach (var timeEffectsPair in LuaAnimClipModel.ListEffect)
                foreach (var effectData in timeEffectsPair.Value)
                    if (effectData.type != AnimClipData.EffectType.Hit && !m_dicIDEffectObject.ContainsKey(effectData.id))
                        CreateEffect(effectData); 
        }

        private static void CreateEffect(AnimClipData.EffectData animClipEffect) {
            EffectConf.EffectData data = LuaEffectConfModel.GetEffectData(animClipEffect.id);
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
            string path = Tool.GetAssetProjectPath(data.resourceName, Config.PrefabExtension, Config.ModelSkillEffectPath);
            GameObject effectNode = LoadPrefab(Tool.ProjectPathToFullPath(path));
            effectNode.transform.SetParent(parent);
            Tool.NormalizeTransform(effectNode);
            m_dicIDEffectObject.Add(animClipEffect.id, effectNode);
            SetEffectTransform(data, effectNode);
            SetEffectParticle(animClipEffect.id, effectNode);
        }

        private static void SetEffectTransform(EffectConf.EffectData data, GameObject effectNode) {
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
                particle.startDelay = 0;
            }
            Animator animator = effectNode.GetComponent<Animator>();
            if (animator == null)
                return;
            AnimatorState state = AnimatorControllerManager.GetAnimatorControllerFirstStateName(animator, Config.ModelSkillEffectPath);
            if (state == null || state.motion == null)
                return;
            SkillAnimator animation = new SkillAnimator(animator);
            animation.Record(state);
            m_dicIDEffectAnimation.Add(id, animation);
        }

        private static bool FilterParticleObject(ParticleSystem particle) {
            var array = System.Enum.GetValues(typeof(Config.EffectFilter));
            for (ushort index = 0; index < array.Length; index++) {
                string componentName = array.GetValue(index).ToString();
                Component component = particle.GetComponent(componentName);
                if (component != null)
                    particle.gameObject.SetActive(false);
                if (!particle.gameObject.activeSelf)
                    break;
            }
            return particle.gameObject.activeSelf;
        }

        private static void SetUseAutoSeedEffect(GameObject rootNode, ParticleSystem particle) {
            var list = Config.UseAutoSeedEffectList;
            foreach (var pair in list) {
                string rootNodeName = pair.Key;
                string particleName = pair.Value;
                if (rootNodeName == rootNode.name && particleName == particle.name) {
                    particle.useAutoRandomSeed = true;
                    break;
                }
            }
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
            ResetDrawCubeData();
            Tool.NormalizeTransform(m_model);
            AnimationModel.SetCurrentAnimationClip(index);
            LuaAnimClipModel.SetCurrentClipName(AnimationModel.SelectAnimationClipName);
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
            StopEffect();
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
            // foreach (var idEffectAnimation in m_dicIDEffectAnimation) {
            //     SkillAnimator animation = idEffectAnimation.Value;
            //     animation.Stop();
            // }
            foreach (var idObjectPair in m_dicIDEffectObject)
                if (idObjectPair.Value.activeSelf)
                    idObjectPair.Value.SetActive(false);
        }

        public static void SetAnimationPlayTime(float time) {
            if (m_modelAnimation.IsPlaying)
                return;
            AnimationClip selectAnimationClip = AnimationModel.SelectAnimationClip;
            m_modelAnimation.SetAnimationPlayTime(selectAnimationClip, time);
            if (m_weaponAnimation != null && !m_isNoWeaponClip)
                m_weaponAnimation.SetAnimationPlayTime(selectAnimationClip, time);
            if (time <= 0)
                StopEffect();
            else
                SetPlayEffectTime(time);            
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
            SetPlayEffectTime((float)(currentTime - m_playStartTime));
            SetDrawCubeData();
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
                    // if (m_dicIDEffectAnimation.ContainsKey(data.id)) {
                    //     SkillAnimator animation = m_dicIDEffectAnimation[data.id];
                    //     animation.SetAnimationPlayTime(sampleTime - time);
                    // }
                }
            }
        }

        private static void SetDrawCubeData() {
            m_listPointCubeData.Clear();
            if (LuaAnimClipModel.ListCollision.Count == 0)
                return;
            float time = m_modelAnimation.PlayTime;
            List<KeyValuePair<float, AnimClipData.CubeData[]>> list = LuaAnimClipModel.ListCollision;
            float minTime = list[0].Key + Config.RuntimeCubeDelay;
            float maxTime = list[list.Count - 1].Key + Config.RuntimeCubeDelay + Config.FramesPerSecond;
            if (time < minTime || time > maxTime)
                return;
            for (int index = 0; index < list.Count; index++) {
                float triggerTime = list[index].Key + Config.RuntimeCubeDelay;
                if (!IsInCollisionTime(time, triggerTime))
                    continue;
                if (!m_dicTimePosition.ContainsKey(triggerTime)) {
                    Transform footTrans = m_model.transform.Find(Config.DrawCubeNodeName);
                    if (footTrans == null)
                        continue;
                    m_dicTimePosition.Add(triggerTime, footTrans.position);
                }
                Vector3 position = m_dicTimePosition[triggerTime];
                foreach (AnimClipData.CubeData data in list[index].Value)
                    m_listPointCubeData.Add(new KeyValuePair<Vector3, AnimClipData.CubeData>(position, data));
            }
            EditorScene.SetDrawCubeData(m_listPointCubeData);
        }

        private static bool IsInCollisionTime(float curTime, float collisionTime) {
            float minTime = collisionTime;
            float maxTime = collisionTime + Config.FramesPerSecond;
            return curTime >= minTime && curTime <= maxTime;
        }

        public static void WriteAnimClipData() => LuaWriter.Write<AnimClipData.AnimClipData>();

        private static void ResetDrawCubeData() {
            m_dicTimePosition.Clear();
            m_listPointCubeData.Clear();
        }

        public static void Reset() {
            LuaAnimClipModel.Reset();
            LuaEffectConfModel.Reset();
            ResetDrawCubeData();
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