using UnityEngine;

namespace SkillEditor {

    internal class SkillClip : BaseAnimation {

        private GameObject m_gameObject;

        private AnimationClip m_curClip;

        public SkillClip(GameObject gameObject) {
            Init(gameObject);
        }

        public override void Init<T>(T gameObject) {
            m_gameObject = gameObject as GameObject;
        }

        public override bool IsPlayOver => m_curPlayTime >= m_curClip.length;
        
        public override void Play(AnimationClip clip) {
            m_curClip = clip;
            Play();
        }

        public override void Stop() {
            base.Stop();
            m_curClip.SampleAnimation(m_gameObject, 0);
        }

        public override void SetAnimationPlayTime(AnimationClip clip, float time) {
            m_curClip = clip;
            m_curPlayTime = time;
            SampleAnimation();
        }

        protected override void SampleAnimation() {
            m_curClip.SampleAnimation(m_gameObject, IsPlayOver ? m_curClip.length : m_curPlayTime);
        }
    }
}