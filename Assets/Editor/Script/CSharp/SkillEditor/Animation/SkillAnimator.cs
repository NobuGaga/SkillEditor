using UnityEngine;

namespace SkillEditor {

    internal static class SkillAnimator {

        private static Animator m_animator;
        public static Animator Animator {
            set {
                if (value == null) {
                    Debug.LogError("SkillAnimator.Animator = null");
                    return;
                }
                Reset();
                m_animator = value;
                m_originPos = m_animator.transform.position;
            }
        }
        private static Vector3 m_originPos;

        private static string m_clipName;
        private static bool m_isPlaying;
        public static bool IsPlayOver => m_isPlaying == false;
        private static float m_curPlayTime;
        private static float m_clipLength;

        private static void Reset() {
            m_clipName = string.Empty;
            m_isPlaying = false;
            m_curPlayTime = 0;
        }

        private static void Bake(AnimationClip clip) {
            float frameRate = clip.frameRate;
            int frameCount = (int)(clip.length * frameRate) + 1;
            m_animator.Rebind();
            m_animator.StopPlayback();
            m_animator.Play(clip.name);
            m_animator.recorderStartTime = 0;
            m_animator.StartRecording(frameCount);
            for (int i = 0; i < frameCount; i++)
                m_animator.Update(1 / frameRate);
            m_animator.StopRecording();
            m_animator.StartPlayback();
            m_clipName = clip.name;
            m_clipLength = clip.length;
        }

        public static void Play(AnimationClip clip) {
            if (clip.name != m_clipName) {
                m_animator.transform.position = m_originPos;
                Bake(clip);
            }
            m_isPlaying = true;
            m_curPlayTime = 0;
        }

        public static void Pause() => m_isPlaying = false;

        public static void Stop() {
            m_isPlaying = false;
            m_curPlayTime = 0;
        }

        public static void Revert() => m_isPlaying = true;

        public static void Update(float deltaTime) {
            if (!m_isPlaying)
                return;
            m_curPlayTime += deltaTime;
            bool isPlayOver = m_curPlayTime >= m_clipLength;
            if (isPlayOver)
                Stop();
            else {
                m_animator.playbackTime = m_curPlayTime;
                m_animator.Update(0);
            }
        }
    }
}