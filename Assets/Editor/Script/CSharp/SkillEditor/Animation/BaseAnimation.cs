using UnityEngine;

namespace SkillEditor {

    internal abstract class BaseAnimation {

        private bool m_isPlaying;
        public virtual bool IsPlayOver => true;
        protected float m_curPlayTime;
        public float PlayTime => m_curPlayTime;
        
        public abstract void Init<T>(T data);

        protected virtual void Play() {
            m_isPlaying = true;
            m_curPlayTime = 0;
        }

        public abstract void Play(AnimationClip clip);

        public virtual void Pause() => m_isPlaying = !m_isPlaying;

        public virtual void Stop() {
            m_isPlaying = false;
        }

        public void Update(float deltaTime) {
            if (!m_isPlaying)
                return;
            m_curPlayTime += deltaTime;
            SampleAnimation();
            if (IsPlayOver)
                Stop();
        }

        protected abstract void SampleAnimation();

        public static bool IsGenericState(AnimationClip clip) {
            return !clip.legacy && !clip.humanMotion;
        }
    }
}