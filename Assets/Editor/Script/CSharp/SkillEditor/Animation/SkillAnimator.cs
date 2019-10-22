using UnityEngine;

namespace SkillEditor {

    internal class SkillAnimator {

        private Animator m_animator;
        public Animator Animator {
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
        private Vector3 m_originPos;

        private string m_clipName;
        private bool m_isPlaying;
        public bool IsPlayOver => m_curPlayTime >= m_clipLength;
        private float m_curPlayTime;
        public float PlayTime => m_curPlayTime;
        private float m_clipLength;

        private void Reset() {
            m_clipName = string.Empty;
            m_isPlaying = false;
            m_curPlayTime = 0;
        }

        private void Bake(AnimationClip clip) {
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

        public void Play(AnimationClip clip) {
            if (clip.name != m_clipName) {
                m_animator.transform.position = m_originPos;
                Bake(clip);
            }
            m_isPlaying = true;
            m_curPlayTime = 0;
        }

        public void Pause() => m_isPlaying = !m_isPlaying;

        public void Stop() {
            m_isPlaying = false;
            m_animator.playbackTime = 0;
            m_animator.Update(0);
        }

        public void Update(float deltaTime) {
            if (!m_isPlaying)
                return;
            m_curPlayTime += deltaTime;
            if (IsPlayOver)
                Stop();
            else {
                m_animator.playbackTime = m_curPlayTime;
                m_animator.Update(0);
            }
        }
    }
}