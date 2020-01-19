using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

namespace SkillEditor {

    internal class WeaponModel {

        private static Dictionary<string, List<WeaponData>> m_dicModelWeapon = new Dictionary<string, List<WeaponData>>();
        private static Dictionary<string, Dictionary<string, AnimationClip>> m_dicModelClip = new Dictionary<string, Dictionary<string, AnimationClip>>();

        private static string m_lastModelName;
        private static string[] m_arrayWeapon;
        private static int[] m_arrayWeaponIndex;

        static WeaponModel() => Init();

        private static void Init() {
            if (!Directory.Exists(Config.WeaponPath))
                return;
            string[] arrayFullPath = Directory.GetDirectories(Config.WeaponPath);
            if (arrayFullPath == null || arrayFullPath.Length == 0)
                return;
            string animControllerFolder = Config.AnimatorControllerFolder;
            for (int index = 0; index < arrayFullPath.Length; index++) {
                string fullPath = arrayFullPath[index];
                if (fullPath.EndsWith(Config.MetaExtension))
                    continue;
                if (fullPath.Contains(animControllerFolder)) {
                    int stringIndex = fullPath.IndexOf(animControllerFolder);
                    if (stringIndex + animControllerFolder.Length >= fullPath.Length)
                        InitAnimatorController(fullPath);
                }
                else if (fullPath.Contains(Config.WeaponFilePrefix)) {
                    string folderName = Tool.GetFileNameFromPath(fullPath);
                    if (folderName.StartsWith(Config.WeaponFilePrefix))
                        InitModelWeapon(folderName, fullPath);
                }
            }
        }

        private static void InitAnimatorController(string path) {
            string[] arrayFullPath = Directory.GetFiles(path);
            if (arrayFullPath == null || arrayFullPath.Length == 0)
                return;
            for (int index = 0; index < arrayFullPath.Length; index++) {
                string fullPath = arrayFullPath[index];
                if (fullPath.EndsWith(Config.MetaExtension))
                    continue;
                string projectPath = Tool.FullPathToProjectPath(fullPath);
                string fileName = Tool.GetFileNameWithourExtensionFromPath(projectPath);
                projectPath = Tool.GetPathFromFilePath(projectPath, fileName);
                AnimatorControllerManager.RemoveAllAnimatorTransition(fileName, projectPath);
            }
        }

        private static void InitModelWeapon(string folderName, string folderFullPath) {
            string prefabPath = Tool.CombinePath(folderFullPath, Config.ModelPrefabFolder);
            if (!Directory.Exists(prefabPath))
                return;
            string[] arrayFullPath = Directory.GetFiles(prefabPath);
            if (arrayFullPath == null || arrayFullPath.Length == 0)
                return;
            string modelName = Tool.GetFileNameWithoutPrefix(folderName, Config.WeaponFilePrefix);
            if (!m_dicModelWeapon.ContainsKey(modelName))
                m_dicModelWeapon.Add(modelName, new List<WeaponData>());
            List<WeaponData> listWeapon = m_dicModelWeapon[modelName];
            listWeapon.Clear();
            for (int index = 0; index < arrayFullPath.Length; index++) {
                string fileFullPath = arrayFullPath[index];
                if (fileFullPath.EndsWith(Config.MetaExtension) || !fileFullPath.EndsWith(Config.PrefabExtension))
                    continue;
                string fileName = Tool.GetFileNameWithourExtensionFromPath(fileFullPath);
                string weaponName = Tool.GetFileNameWithoutPrefix(fileName, Config.WeaponFilePrefix);
                WeaponData data = new WeaponData();
                data.weaponName = weaponName;
                data.weaponPath = Tool.FullPathToProjectPath(fileFullPath);
                listWeapon.Add(data);
            }
            InitModelWeaponClip(modelName, folderFullPath);
        }

        private static void InitModelWeaponClip(string modelName, string folderFullPath) {
            if (m_dicModelClip.ContainsKey(modelName))
                return;
            string clipPath = Tool.CombinePath(folderFullPath, Config.AnimationClipFolder);
            if (!Directory.Exists(clipPath))
                return;
            string[] arrayFullPath = Directory.GetFiles(clipPath);
            if (arrayFullPath == null || arrayFullPath.Length == 0)
                return;
            Dictionary<string, AnimationClip> dicClip = null;
            for (int index = 0; index < arrayFullPath.Length; index++) {
                string fileFullPath = arrayFullPath[index];
                if (fileFullPath.EndsWith(Config.MetaExtension) || !fileFullPath.Contains(Config.AnimationClipSymbol) ||
                    !fileFullPath.Contains(Config.ClipUpperExtension))
                    continue;
                string path = Tool.FullPathToProjectPath(fileFullPath);
                AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
                if (clip == null)
                    continue;
                if (dicClip == null)
                    dicClip = new Dictionary<string, AnimationClip>();
                if (dicClip.ContainsKey(clip.name))
                    dicClip[clip.name] = clip;
                else
                    dicClip.Add(clip.name, clip);
            }
            if (dicClip != null && dicClip.Count != 0)
                m_dicModelClip.Add(modelName, dicClip);
        }

        public static string[] GetAllWeaponName(string modelName) {
            if (m_lastModelName != null && modelName == m_lastModelName)
                return m_arrayWeapon;
            m_arrayWeapon = null;
            if (!m_dicModelWeapon.ContainsKey(modelName))
                return null;
            List<WeaponData> list = m_dicModelWeapon[modelName];
            if (list == null || list.Count == 0)
                return null;
            m_arrayWeapon = new string[list.Count];
            for (int index = 0; index < list.Count; index++)
                m_arrayWeapon[index] = list[index].weaponName;
            return m_arrayWeapon;
        }

        public static int[] GetAllWeaponNameIndex(string modelName) {
            if (m_lastModelName != null && modelName == m_lastModelName)
                return m_arrayWeaponIndex;
            m_arrayWeaponIndex = null;
            string[] array = GetAllWeaponName(modelName);
            if (array == null || array.Length == 0)
                return m_arrayWeaponIndex;
            m_arrayWeaponIndex = new int[array.Length];
            for (int index = 0; index < array.Length; index++)
                m_arrayWeaponIndex[index] = index;
            return m_arrayWeaponIndex;
        }

        public static string GetWeaponPrefabPath(string modelName, int index) {
            if (!m_dicModelWeapon.ContainsKey(modelName))
                return null;
            List<WeaponData> list = m_dicModelWeapon[modelName];
            if (list == null || index == Config.ErrorIndex || index >= list.Count)
                return null;
            return list[index].weaponPath;
        }

        public static bool CheckModelHasWeaponClip(string modelName) {
            return m_dicModelClip.ContainsKey(modelName);
        }

        public static AnimationClip GetAnimationClip(string modelName, string clipName) {
            if (!CheckModelHasWeaponClip(modelName))
                return null;
            Dictionary<string, AnimationClip> dicClip = m_dicModelClip[modelName];
            if (!dicClip.ContainsKey(clipName))
                return null;
            return dicClip[clipName];
        }
 
        public static bool GetGenericState(string modelName) {
            Dictionary<string, AnimationClip> dicClip = m_dicModelClip[modelName];
            AnimationClip sampleClip = null;
            foreach (var keyValue in dicClip) {
                sampleClip = keyValue.Value;
                break;
            }
            return BaseAnimation.IsGenericState(sampleClip);
        }

        private static void Clear() {
            m_dicModelWeapon.Clear();
            m_dicModelClip.Clear();
            m_lastModelName = null;
            m_arrayWeapon = null;
            m_arrayWeaponIndex = null;
        }

        public static void Refresh() {
            Clear();
            Init();
        }

        private struct WeaponData {
            
            public string weaponName;
            public string weaponPath;
        }
    }
}