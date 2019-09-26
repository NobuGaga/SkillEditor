using UnityEngine;

public static class SkillEditorData {

    private static string[] m_aAniamtionClipNames;
    public static string[] AnimationClipsNames => m_aAniamtionClipNames;

    private static AnimationClip[] m_aAniamtionClips;

    public static void Set(string[] clipNames, AnimationClip[] clips) {
        m_aAniamtionClipNames = clipNames;
        m_aAniamtionClips = clips;
    }

    public static AnimationClip GetAnimationClip(string clipName) {
        for (int i = 0; i < m_aAniamtionClips.Length; i++)
            if (clipName == m_aAniamtionClips[i].name)
                return m_aAniamtionClips[i];
        return null;
    }
}