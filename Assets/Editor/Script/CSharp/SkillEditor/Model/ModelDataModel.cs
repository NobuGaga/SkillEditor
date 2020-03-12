using UnityEngine;
using System.Collections.Generic;

namespace SkillEditor {

    internal class ModelDataModel {

        private static Dictionary<string, ModelData> m_dicModelPathData = new Dictionary<string, ModelData>();
        public static string CurrentPrefabPath;
        private static ModelData CurrentModelData {
            get {
                if (string.IsNullOrEmpty(CurrentPrefabPath))
                    return default;
                if (m_dicModelPathData.ContainsKey(CurrentPrefabPath))
                    return m_dicModelPathData[CurrentPrefabPath];
                return default;
            }
        }

        public static string ModelName => CurrentModelData.modelName;
        public static string ClipFullPath => CurrentModelData.clipFullPath;
        public static string ControllerPath => CurrentModelData.controllerProjectPath;
        public static void SetPrefabFullPath(string prefabFullPath) {
            if (m_dicModelPathData.ContainsKey(prefabFullPath))
                return;
            CurrentPrefabPath = prefabFullPath;
            ModelData data = new ModelData();
            data.prefabFullPath = prefabFullPath;

            int endIndex = prefabFullPath.IndexOf("/prefabs/");
            if (endIndex == Config.ErrorIndex)
                Debug.LogError("选择模型预设路径错误");
            int startIndex = prefabFullPath.LastIndexOf('/', endIndex - 1);
            if (startIndex == Config.ErrorIndex)
                Debug.LogError("选择模型预设路径错误");
            data.modelName = prefabFullPath.Substring(startIndex + 1, endIndex - startIndex - 1);
            if (data.modelName.Contains(Config.ModelPrefabPrefix))
                data.modelName = data.modelName.Substring(Config.ModelPrefabPrefix.Length);
            string modelFileGroupFullPath = prefabFullPath.Substring(0, endIndex);
            data.clipFullPath = Tool.CombinePath(modelFileGroupFullPath, "models");
            string controllerPath = Tool.CombinePath(modelFileGroupFullPath, Config.AnimatorControllerFolder);
            data.controllerProjectPath = Tool.FullPathToProjectPath(controllerPath);
            m_dicModelPathData.Add(CurrentPrefabPath, data);
        }

        public static void Clear() {
            m_dicModelPathData.Clear();
            CurrentPrefabPath = null;
        }

        public static void Refresh() => Clear();

        #pragma warning disable 0649
        private struct ModelData {

            public string modelName;
            public string prefabName;
            public string prefabFullPath;
            public string clipFullPath;
            public string controllerProjectPath;
        }
        #pragma warning restore 0649
    }
}