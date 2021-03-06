﻿using UnityEngine;
using System.Collections.Generic;

namespace SkillEditor {

    internal static class AnimationModel {

        private static AnimationClip[] m_aniamtionClips;
        public static AnimationClip[] AnimationClips {
            set {
                m_aniamtionClips = value;
                m_listAnimationClipNames.Clear();
                m_aAnimationClipNames = null;
                if (m_aniamtionClips == null || m_aniamtionClips.Length == 0) {
                    Debug.LogError("SkillEditor.AnimationModel::SetAnimationClips value is empty");
                    return;
                }
                for (int i = 0; i < m_aniamtionClips.Length; i++)
                    m_listAnimationClipNames.Add(m_aniamtionClips[i].name);
                m_aAnimationClipNames = m_listAnimationClipNames.ToArray();
                if (m_aniamtionClips.Length > m_listAnimationClipIndexs.Count) {
                    for (int i = m_listAnimationClipIndexs.Count; i < m_aniamtionClips.Length; i++)
                        m_listAnimationClipIndexs.Add(i);
                }
                else if (m_aniamtionClips.Length < m_listAnimationClipIndexs.Count)
                    m_listAnimationClipIndexs.RemoveRange(m_aAnimationClipNames.Length, 
                                                            m_listAnimationClipIndexs.Count - m_aniamtionClips.Length);
                m_aAnimationClipIndexs = m_listAnimationClipIndexs.ToArray();
            } 
            get { return m_aniamtionClips; }
        }

        public static bool GenericState() {
            if (m_aniamtionClips == null)
                return false;
            AnimationClip sampleClip = m_aniamtionClips[0];
            return BaseAnimation.IsGenericState(sampleClip);
        }

        private static List<string> m_listAnimationClipNames = new List<string>(16);
        private static string[] m_aAnimationClipNames;
        public static string[] AnimationClipNames {
            get { return m_aAnimationClipNames; }
        }

        private static List<int> m_listAnimationClipIndexs = new List<int>(16);
        private static int[] m_aAnimationClipIndexs;
        public static int[] AnimationClipIndexs {
            get { return m_aAnimationClipIndexs; }
        }

        private static string m_selectAnimationClipName;
        public static string SelectAnimationClipName => m_selectAnimationClipName;
        private static AnimationClip m_selectAnimationClip;
        public static AnimationClip SelectAnimationClip => m_selectAnimationClip;
        public static float SelectAnimationClipTime => m_selectAnimationClip == null ? 0 : m_selectAnimationClip.length;

        public static void SetCurrentAnimationClip(int index) {
            if (m_aniamtionClips == null || m_aniamtionClips.Length == 0)
                Debug.LogError("SkillEditor.AnimationModel::GetAnimationClip animationClips is empty");
            if (index < 0 || index >= m_aniamtionClips.Length)
                Debug.LogError("SkillEditor.AnimationModel::GetAnimationClip argument index is invalid");
            m_selectAnimationClipName = m_listAnimationClipNames[index];
            m_selectAnimationClip = m_aniamtionClips[index];
        }
    }
}