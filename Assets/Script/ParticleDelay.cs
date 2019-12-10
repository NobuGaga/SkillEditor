using UnityEngine;

namespace SkillEditor {

    public class ParticleDelay : MonoBehaviour {

        private const string TipString = "粒子特效偏移值";

        [Tooltip(TipString)][Header(TipString)][SerializeField]
        private float m_delayTimeOffset = 0;
        public float DelayTimeOffset => m_delayTimeOffset;
    }
}