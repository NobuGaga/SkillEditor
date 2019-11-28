using UnityEngine;
using System.Collections.Generic;

namespace SkillEditor {

    internal class ModelDataModel {

        private static Dictionary<string, ModelData> m_dicModelPathData = new Dictionary<string, ModelData>();
        public static string CurrentPrefabPath;
        private static ModelData CurrentModelData => m_dicModelPathData[CurrentPrefabPath];
        public static string ModelName => CurrentModelData.modelName;
        public static string ClipFullPath => CurrentModelData.clipFullPath;
        public static string ControllerPath => CurrentModelData.controllerProjectPath;
        public static void SetPrefabFullPath(string prefabFullPath) {
            if (m_dicModelPathData.ContainsKey(prefabFullPath))
                return;
            CurrentPrefabPath = prefabFullPath;
            ModelData data = new ModelData();
            data.prefabFullPath = prefabFullPath;

            int subIndex = prefabFullPath.IndexOf(Config.ModelPrefabPrefix);
            if (subIndex == Config.ErrorIndex) {
                Debug.LogError("选择模型预设路径错误");
                return;
            }
            subIndex += Config.ModelPrefabPrefix.Length;
            int endIndex = prefabFullPath.IndexOf('\\', subIndex);
            if (endIndex == Config.ErrorIndex) {
                Debug.LogError("选择模型预设路径错误");
                return;
            }
            data.prefabName = prefabFullPath.Substring(subIndex, endIndex);
            subIndex = prefabFullPath.IndexOf("prefabs/");
            string modelFileGroupFullPath = prefabFullPath.Substring(0, subIndex);
            data.clipFullPath = Tool.CombinePath(modelFileGroupFullPath, "models");
            string controllerPath = Tool.CombinePath(modelFileGroupFullPath, Config.AnimatorControllerFolder);
            data.controllerProjectPath = Tool.FullPathToProjectPath(controllerPath);
        }

        public static void Clear() => CurrentPrefabPath = null;

        public static void Refresh() => Clear();

        private struct ModelData {

            public string modelName;
            public string prefabName;
            public string prefabFullPath;
            public string clipFullPath;
            public string controllerProjectPath;

            public ModelData(string modelName, string prefabName, string prefabFullPath, string clipFullPath, string controllerProjectPath) {
                this.modelName = modelName;
                this.prefabName = prefabName;
                this.prefabFullPath = prefabFullPath;
                this.clipFullPath = clipFullPath;
                this.controllerProjectPath = controllerProjectPath;
            }
        }
    }
}