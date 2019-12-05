using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

namespace SkillEditor {

    internal static class AnimatorControllerManager {
        
        private const string AnimatorControllerCopyName = "Copy";

        private static Dictionary<string, string> m_dicFile = new Dictionary<string, string>();

        public static void RemoveAllAnimatorTransition(string fileName, string fileProjectPath) {
            if (m_dicFile.ContainsKey(fileName))
                return;
            string sourceProjectPath = Tool.CombineFilePath(fileProjectPath, fileName, Config.AnimatorControllerExtension);
            string fileFullPath = Tool.ProjectPathToFullPath(fileProjectPath);
            string sourceFullPath = Tool.CombineFilePath(fileFullPath, fileName, Config.AnimatorControllerExtension);
            string copyPath = Tool.CombineFilePath(fileFullPath, fileName + AnimatorControllerCopyName, Config.AnimatorControllerExtension);
            File.Copy(sourceFullPath, copyPath, true);
            m_dicFile.Add(fileName, fileFullPath);
            AnimatorController animatorController = AssetDatabase.LoadAssetAtPath<AnimatorController>(sourceProjectPath);
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

        public static void RevertAnimatorControllerFile() {
            foreach (KeyValuePair<string, string> keyValuePair in m_dicFile) {
                string fileName = keyValuePair.Key;
                string fileFullPath = keyValuePair.Value;
                string copyPath = Tool.CombineFilePath(fileFullPath, fileName + AnimatorControllerCopyName, Config.AnimatorControllerExtension);
                if (!File.Exists(copyPath))
                    continue;
                string sourcePath = Tool.CombineFilePath(fileFullPath, fileName, Config.AnimatorControllerExtension);
                File.Copy(copyPath, sourcePath, true);
                File.Delete(copyPath);
                string copyMetaPath = Tool.FileWithExtension(copyPath, Config.MetaExtension);
                if (!File.Exists(copyMetaPath))
                    continue;
                File.Delete(copyMetaPath);
            }
            m_dicFile.Clear();
            AssetDatabase.SaveAssets();
        }

        public static AnimatorState GetAnimatorControllerFirstStateName(Animator animator, string[] folders = null) {
            if (animator.runtimeAnimatorController == null) {
                Debug.LogError("AnimatorControllerManager::GetFirstState animator has no controller file");
                return null;
            }
            string controllerName = animator.runtimeAnimatorController.name;
            string path = Tool.GetAssetProjectPath(controllerName, folders);
            AnimatorController animatorController = AssetDatabase.LoadAssetAtPath<AnimatorController>(path);
            AnimatorControllerLayer[] layers = animatorController.layers;
            for (int layerIndex = 0; layerIndex < layers.Length; layerIndex++) {
                AnimatorStateMachine mainMachine = layers[layerIndex].stateMachine;
                if (mainMachine == null || mainMachine.states == null)
                    continue;
                for (int stateIndex = 0; stateIndex < mainMachine.states.Length; stateIndex++) {
                    AnimatorState state = mainMachine.states[stateIndex].state;
                    if (state != null)
                        return state;
                }
            }
            return null;
        }
    }
}