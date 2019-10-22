using UnityEngine;

namespace SkillEditor {

    internal class SkillClip {

        private GameObject m_gameObject;
        public GameObject ModelGameObject {
            set {
                if (value == null) {
                    Debug.LogError("SkillClip.ModelGameObject = null");
                    return;
                }
                m_gameObject = value;
            }
        }

        private AnimationClip m_curClip;
        public AnimationClip Clip {
            set {
                if (value == null) {
                    Debug.LogError("SkillClip.Clip = null");
                    return;
                }
                m_curClip = value;
            }
        }

        private bool m_isPlaying;
        public bool IsPlayOver => m_curPlayTime >= m_curClip.length;
        private float m_curPlayTime;
        public float PlayTime => m_curPlayTime;
        
        public void Play() {
            m_isPlaying = true;
            m_curPlayTime = 0;
        }

        public void Play(AnimationClip clip) {
            Clip = clip;
            Play();
        }

        public void Play(GameObject gameObject, AnimationClip clip) {
            ModelGameObject = gameObject;
            Play(clip);
        }

        public void Pause() => m_isPlaying = !m_isPlaying;

        public void Stop() {
            m_isPlaying = false;
            m_curClip.SampleAnimation(m_gameObject, 0);
        }

        public void Update(float deltaTime) {
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