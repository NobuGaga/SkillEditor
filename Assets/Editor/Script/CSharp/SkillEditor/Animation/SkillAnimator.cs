using UnityEngine;

namespace SkillEditor {

    internal class SkillAnimator : BaseAnimation {

        private Animator m_animator;
        private Vector3 m_originPos;
        private string m_clipName;
        public override bool IsPlayOver => m_curPlayTime >= m_clipLength;
        private float m_clipLength;

        public SkillAnimator(Animator animator) {
            Init(animator);
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

        public override void Init<T>(T animator) {
            m_animator = animator as Animator;
            m_originPos = m_animator.transform.position;
        }

        public override void Play(AnimationClip clip) {
            if (clip.name != m_clipName) {
                m_animator.transform.position = m_originPos;
                Bake(clip);
            }
            Play();
        }

        public override void Stop() {
            base.Stop();
            m_animator.playbackTime = 0;
            m_animator.Update(0);
        }

        protected override void SampleAnimation() {
            if (IsPlayOver)
                return;
            m_animator.playbackTime = m_curPlayTime;
            m_animator.Update(0);
        }
    }
}