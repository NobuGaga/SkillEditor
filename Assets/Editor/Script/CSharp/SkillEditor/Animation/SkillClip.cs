using UnityEngine;

namespace SkillEditor {

    internal static class SkillClip {

        private static GameObject m_gameObject;
        public static GameObject ModelGameObject {
            set {
                if (value == null) {
                    Debug.LogError("SkillClip.ModelGameObject = null");
                    return;
                }
                m_gameObject = value;
            }
        }

        private static AnimationClip m_curClip;
        public static AnimationClip Clip {
            set {
                if (value == null) {
                    Debug.LogError("SkillClip.Clip = null");
                    return;
                }
                m_curClip = value;
            }
        }

        private static bool m_isPlaying;
        public static bool IsPlayOver => m_curPlayTime >= m_curClip.length;
        private static float m_curPlayTime;
        public static float PlayTime => m_curPlayTime;
        
        public static void Play() {
            m_isPlaying = true;
            m_curPlayTime = 0;
        }

        public static void Play(AnimationClip clip) {
            Clip = clip;
            Play();
        }

        public static void Play(GameObject gameObject, AnimationClip clip) {
            ModelGameObject = gameObject;
            Play(clip);
        }

        public static void Pause() => m_isPlaying = !m_isPlaying;

        public static void Stop() {
            m_isPlaying = false;
            m_curClip.SampleAnimation(m_gameObject, 0);
        }

        public static void Update(float deltaTime) {
            if (!m_isPlaying)
                return;
            m_curPlayTime += deltaTime;
            bool isPlayOver = IsPlayOver;
            m_curClip.SampleAnimation(m_gameObject, isPlayOver ? m_curClip.length : m_curPlayTime);
            if (isPlayOver)
                Stop();
        }
    }
}