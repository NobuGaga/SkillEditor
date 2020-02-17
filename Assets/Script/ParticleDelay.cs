using UnityEngine;

namespace SkillEditor {

    public class ParticleDelay : MonoBehaviour {

        private const string InspectorName = "粒子特效偏移值";

        [SerializeField]
        [InspectorName(InspectorName)]
        private float m_delayTimeOffset = 0;
        public float DelayTimeOffset => m_delayTimeOffset;
    }
}