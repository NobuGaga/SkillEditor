using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public static class SkillEditorController {

    private static GameObject m_model = null;

    private static List<AnimationClip> m_animationClips = new List<AnimationClip>(SkillEditorConfig.DefaultAnimationClipLength);

    private static bool m_isGenericClip;

    private static double m_lastTime;

    public static void Start(string prefabPath) {
        if (m_model)
            Object.DestroyImmediate(m_model);
        GameObject prefab = PrefabUtility.LoadPrefabContents(prefabPath);
        m_model = Object.Instantiate(prefab);
        m_model.name = prefab.name;
        Selection.activeGameObject = m_model;
        PrefabUtility.UnloadPrefabContents(prefab);
        SetAllAnimationClip();
        SkillEditorData.AnimationClips = m_animationClips.ToArray();
        m_isGenericClip = SkillEditorData.GenericState();
        InitAnimation();
        SkillEditorScene.RegisterSceneGUI();
        SkillEditorWindow.SetDisplayData(SkillEditorData.AnimationClipNames, SkillEditorData.AnimationClipIndexs);
        SkillEditorWindow.Open();
    }

    private static void SetAllAnimationClip() {
        string[] fileNames = Directory.GetFiles(SkillEditorConfig.ClipGroupFullPath);
        m_animationClips.Clear();
        for (int index = 0; index < fileNames.Length; index++) {
            if (fileNames[index].Contains(".meta") || !fileNames[index].Contains("@") ||
                !(fileNames[index].Contains(".fbx") || fileNames[index].Contains(".FBX")))
                continue;
            string path = SkillEditorTool.FullPathToProjectPath(fileNames[index]);
            AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
            if (clip == null)
                continue;
            m_animationClips.Add(clip);
        }
    }

    private static void InitAnimation() {
        if (!m_isGenericClip)
            return;
        Animator animator = m_model.GetComponent<Animator>();
        if (animator == null)
            Debug.LogError("Prefab's animator is not exit");
        SkillEditorAnimator.Animator = animator;
        RemoveAllAnimatorTransition();
    }

    private static void RemoveAllAnimatorTransition() {
        AnimatorController animatorCtrl = AssetDatabase.LoadAssetAtPath<AnimatorController>(SkillEditorConfig.ControllerPath);
        AnimatorControllerLayer[] layers = animatorCtrl.layers;
        for (int layerIndex = 0; layerIndex < layers.Length; layerIndex++) {
            AnimatorStateMachine machine = layers[layerIndex].stateMachine;
            if (machine == null)
                continue;
            ChildAnimatorState[] states = machine.states;
            if (states == null)
                continue;
            for (int stateIndex = 0; stateIndex < states.Length; stateIndex++) {
                AnimatorState state = states[stateIndex].state;
                if (state == null || state.transitions == null)
                    continue;
                int transIndex = state.transitions.Length - 1;
                for (; transIndex >= 0; transIndex--)
                    state.RemoveTransition(state.transitions[transIndex]);
            }
        }
    }

    public static void SetAnimationClipData(int index) {
        SkillEditorData.SetCurrentAnimationClip(index);
    }

    public static void Play() {
        m_lastTime = EditorApplication.timeSinceStartup;
        EditorApplication.update += Update;
        if (m_isGenericClip)
            SkillEditorAnimator.Play(SkillEditorData.SelectAnimationClip);
        else
            SkillEditorClip.Play(m_model, SkillEditorData.SelectAnimationClip);
    }

    private static void Update() {
        if ((m_isGenericClip && SkillEditorAnimator.IsPlayOver) || (!m_isGenericClip && SkillEditorClip.IsPlayOver))
            Stop();
        double currentTime = EditorApplication.timeSinceStartup;
        float deltaTime = (float)(currentTime - m_lastTime);
        m_lastTime = currentTime;
        if (m_isGenericClip)
            SkillEditorAnimator.Update(deltaTime);
        else
            SkillEditorClip.Update(deltaTime);
    }

    public static void Stop() {
        EditorApplication.update -= Update;
    }

    public static void Exit() {
        m_model = null;
    }
}