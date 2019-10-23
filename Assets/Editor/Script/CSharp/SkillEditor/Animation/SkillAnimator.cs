using UnityEngine;

namespace SkillEditor {

    internal class SkillAnimator : BaseAnimation {

        private Animator m_animator;
        private Vector3 m_originPos;
        private string m_clipName;

        public SkillAnimator(Animator animator) {
            Init(animator);
        }

        public override void Init<T>(T animator) {
            m_animator = animator as Animator;
            m_originPos = m_animator.transform.position;
        }

        private void CheckHasRecord(AnimationClip clip) {
            if (clip.name == m_clipName)
                return;
            m_clipName = clip.name;
            Record(clip);
        }

        private void Record(AnimationClip clip) {
            float frameRate = clip.frameRate;
            int frameCount = (int)(clip.length * frameRate) + 1;
            m_animator.Rebind();
            m_animator.StopPlayback();
            m_animator.Play(clip.name);
            m_animator.recorderStartTime = 0;
            m_animator.StartRecording(frameCount);
            for (int index = 0; index < frameCount; index++)
                m_animator.Update(1 / frameRate);
            m_animator.StopRecording();
            m_animator.StartPlayback();
        }

        public override void Play(AnimationClip clip) {
            CheckHasRecord(clip);
            m_animator.transform.position = m_originPos;
            base.Play(clip);
        }

        public override void SetAnimationPlayTime(AnimationClip clip, float time) {
            CheckHasRecord(clip);
            base.SetAnimationPlayTime(clip, time);
        }

        protected override void SampleAnimation() {
            m_animator.playbackTime = m_curPlayTime;
            m_animator.Update(0);
        }
    }
}