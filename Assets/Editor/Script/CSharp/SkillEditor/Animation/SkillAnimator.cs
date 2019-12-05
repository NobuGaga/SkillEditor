using UnityEditor.Animations;
using UnityEngine;

namespace SkillEditor {

    internal class SkillAnimator : BaseAnimation {

        private Animator m_animator;
        private string m_playName;

        public SkillAnimator(Animator animator) => Init(animator);

        public override void Init<T>(T animator) => m_animator = animator as Animator;

        private void CheckRecord(AnimationClip clip) {
            if (clip.name == m_playName)
                return;
            Record(clip);
        }

        private void CheckRecord(AnimatorState state) {
            if (state.name == m_playName)
                return;
            Record(state);
        }

        private void Record(AnimationClip clip) {
            m_playName = clip.name;
            Record(clip.name, clip.length, clip.frameRate);
        }

        public void Record(AnimatorState state) {
            m_playName = state.name;
            Record(state.name, state.motion.averageDuration, Config.FrameCount);
        }

        private void Record(string name, float length, float frameRate) {
            int frameCount = (int)(length * frameRate) + 1;
            m_animator.Rebind();
            m_animator.StopPlayback();
            m_animator.Play(name);
            m_animator.recorderStartTime = 0;
            m_animator.StartRecording(frameCount);
            for (int index = 0; index < frameCount; index++)
                m_animator.Update(1 / frameRate);
            m_animator.StopRecording();
            m_animator.StartPlayback();
        }

        public override void Play(AnimationClip clip) {
            CheckRecord(clip);
            base.Play(clip);
        }

        public override void SetAnimationPlayTime(AnimationClip clip, float time) {
            CheckRecord(clip);
            base.SetAnimationPlayTime(clip, time);
        }

        public void SetAnimationPlayTime(float time) {
            if (string.IsNullOrEmpty(m_playName)) {
                Debug.LogError("SkillAnimator::Play animator hasn't record before calling play function");
                return;
            }
            base.SetAnimationPlayTime(null, time);
        }

        protected override void SampleAnimation() {
            m_animator.playbackTime = m_curPlayTime;
            m_animator.Update(0);
        }
    }
}